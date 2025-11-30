#include "WaitingRoom.h"
#include "CFG.h"
#include "Core.h"
#include <iostream>
#include <fstream>
#include <vector>
#include <string>
#include <Windows.h> 
#include <sstream>


/* ******************************************** */

WaitingRoom::WaitingRoom(void) {
	this->lMO.push_back(new MenuOption("PLAY", 40, 276));
	this->lMO.push_back(new MenuOption("CHAT", 40, 308));
	this->lMO.push_back(new MenuOption("ESCAPE", 40, 340));



	this->numOfMenuOptions = lMO.size();

	this->selectWorld = this->selectChat = this->typing = this->host = false;
	runChatThread = false;

	rSelectWorld.x = 122;
	rSelectWorld.y = 280;
	rSelectWorld.w = 306;
	rSelectWorld.h = 72;

	this->activeWorldID = this->activeSecondWorldID = 0;
	
	playerNames.clear();
	chatMessages.clear();
}

WaitingRoom::~WaitingRoom(void) {
	runPlayerThread = false;
	if (playerThread.joinable()) playerThread.join();

	runChatThread = false;
}

/* ******************************************** */

std::vector<std::string> ParseMessages(const std::string& raw)
{
	std::vector<std::string> out;
	if (raw.size() < 2) return out;

	// Bỏ dấu " bên ngoài
	std::string s = raw.substr(1, raw.size() - 2);

	size_t start = 0;
	while (true) {
		size_t quote1 = s.find('"', start);
		if (quote1 == std::string::npos) break;

		size_t quote2 = s.find('"', quote1 + 1);
		if (quote2 == std::string::npos) break;

		out.push_back(s.substr(quote1 + 1, quote2 - quote1 - 1));

		start = quote2 + 1;
	}

	return out;
}

static size_t WriteCallback(void* contents, size_t size, size_t nmemb, void* userp)
{
	((std::string*)userp)->append((char*)contents, size * nmemb);
	return size * nmemb;
}

std::string GetMessagesFromAPI(std::string idRoom, std::string antiPlayerName) {
	CURL* curl = curl_easy_init();
	std::string responseString;
	if (!curl) return responseString;

	// URL gồm Id và playerName
	std::string url = "https://localhost:7244/api/Rooms/getMessages/" + idRoom;

	curl_easy_setopt(curl, CURLOPT_URL, url.c_str());

	curl_easy_setopt(curl, CURLOPT_WRITEFUNCTION, WriteCallback);
	curl_easy_setopt(curl, CURLOPT_WRITEDATA, &responseString);

	curl_easy_setopt(curl, CURLOPT_SSL_VERIFYPEER, 0L);
	curl_easy_setopt(curl, CURLOPT_SSL_VERIFYHOST, 0L);

	CURLcode res = curl_easy_perform(curl);

	curl_easy_cleanup(curl);
	return responseString;
}

void CallEscapeRoomAPI(std::string idRoom, std::string userName) {
	CURL* curl = curl_easy_init();
	if (!curl) {
		return;
	}

	std::string url = "https://localhost:7244/api/Rooms/" + idRoom + "?playerName=" + userName;

	curl_easy_setopt(curl, CURLOPT_URL, url.c_str());

	curl_easy_setopt(curl, CURLOPT_CUSTOMREQUEST, "DELETE");

	curl_easy_setopt(curl, CURLOPT_WRITEFUNCTION, WriteCallback);

	curl_easy_setopt(curl, CURLOPT_SSL_VERIFYPEER, 0L);
	curl_easy_setopt(curl, CURLOPT_SSL_VERIFYHOST, 0L);

	CURLcode res = curl_easy_perform(curl);

	/*if (res != CURLE_OK) {
		std::cerr << "Curl failed: " << curl_easy_strerror(res) << std::endl;
	}*/

	curl_easy_cleanup(curl);
}

std::string GetPlayersAPI(std::string idRoom) {
	CURL* curl = curl_easy_init();
	std::string responseString;

	if (!curl) return "";

	std::string url = "https://localhost:7244/api/Rooms/Players?Id=" + idRoom;

	curl_easy_setopt(curl, CURLOPT_URL, url.c_str());
	curl_easy_setopt(curl, CURLOPT_WRITEFUNCTION, WriteCallback);
	curl_easy_setopt(curl, CURLOPT_WRITEDATA, &responseString);

	curl_easy_setopt(curl, CURLOPT_SSL_VERIFYPEER, 0L);
	curl_easy_setopt(curl, CURLOPT_SSL_VERIFYHOST, 0L);

	curl_easy_perform(curl);
	curl_easy_cleanup(curl);

	return responseString;
}

void WaitingRoom::ChatFetchThread(std::string idRoom, std::string userName, std::vector<std::string>& chatMessages) {
	while (runChatThread) {
		std::string raw = GetMessagesFromAPI(idRoom, userName);
		auto list = ParseMessages(raw);

		{
			std::lock_guard<std::mutex> lock(chatMutex);
			chatMessages = list; 
		}
		std::this_thread::sleep_for(std::chrono::milliseconds(200));
	}
}

void WaitingRoom::Update() {
	Menu::Update();

	std::ifstream file("room_info.txt");
	if (file.is_open()) {
		std::getline(file, idRoom);
		file.close();
	}

	if (!runPlayerThread) {
		runPlayerThread = true;

		if (playerThread.joinable()) playerThread.join();

		playerThread = std::thread([this]() { this->PlayerFetchThread(idRoom); });
	}
}

void WaitingRoom::Draw(SDL_Renderer* rR) {
	SDL_Rect playerBox;
	playerBox.x = 20;
	playerBox.y = 80;
	playerBox.w = 200;
	playerBox.h = 180;

	SDL_SetRenderDrawBlendMode(rR, SDL_BLENDMODE_BLEND);
	SDL_SetRenderDrawColor(rR, 0, 0, 0, 170); 
	SDL_RenderFillRect(rR, &playerBox);

	SDL_SetRenderDrawColor(rR, 255, 255, 255, 255);
	SDL_RenderDrawRect(rR, &playerBox);

	SDL_SetRenderDrawBlendMode(rR, SDL_BLENDMODE_NONE);

	Menu::Draw(rR);

	CCFG::getText()->Draw(rR, "MEMBERS:", 50, 100, 18, 255, 255, 255);

	std::vector<std::string> namesCopy;
	{
		std::lock_guard<std::mutex> lock(playerMutex);
		namesCopy = playerNames;
	}

	int startY = 150;
	for (const auto& name : namesCopy) {
		CCFG::getText()->Draw(rR, name, 40, startY, 14, 255, 255, 255);
		startY += 22;
	}

	if (selectWorld) {
		SDL_SetRenderDrawBlendMode(rR, SDL_BLENDMODE_BLEND);
		SDL_SetRenderDrawColor(rR, 4, 4, 4, 235);
		SDL_RenderFillRect(rR, &rSelectWorld);
		SDL_SetRenderDrawColor(rR, 255, 255, 255, 255);
		rSelectWorld.x += 1;
		rSelectWorld.y += 1;
		rSelectWorld.h -= 2;
		rSelectWorld.w -= 2;
		SDL_RenderDrawRect(rR, &rSelectWorld);
		rSelectWorld.x -= 1;
		rSelectWorld.y -= 1;
		rSelectWorld.h += 2;
		rSelectWorld.w += 2;

		CCFG::getText()->Draw(rR, "SELECT WORLD", rSelectWorld.x + rSelectWorld.w / 2 - CCFG::getText()->getTextWidth("SELECT WORLD") / 2, rSelectWorld.y + 16, 16, 255, 255, 255);

		for (int i = 0, extraX = 0; i < 8; i++) {
			if (i == activeWorldID) {
				CCFG::getText()->Draw(rR, std::to_string(i + 1) + "-" + std::to_string(activeSecondWorldID + 1), rSelectWorld.x + 16 * (i + 1) + 16 * i + extraX, rSelectWorld.y + 16 + 24, 16, 255, 255, 255);

				extraX = 32;

				/*for(int j = 0; j < 4; j++) {
					if(j == activeSecondWorldID) {
						CCFG::getText()->Draw(rR, std::to_string(j + 1), rSelectWorld.x + 16*(i + 1) + 16*i, rSelectWorld.y + 40 + 24*j, 16, 255, 255, 255);
					} else {
						CCFG::getText()->Draw(rR, std::to_string(j + 1), rSelectWorld.x + 16*(i + 1) + 16*i, rSelectWorld.y + 40 + 24*j, 16, 90, 90, 90);
					}
				}*/
			}
			else {
				CCFG::getText()->Draw(rR, std::to_string(i + 1), rSelectWorld.x + 16 * (i + 1) + 16 * i + extraX, rSelectWorld.y + 16 + 24, 16, 90, 90, 90);
			}
		}

		SDL_SetRenderDrawBlendMode(rR, SDL_BLENDMODE_NONE);
		CCore::getMap()->setBackgroundColor(rR);
	}

	if (selectChat) {
		SDL_Rect chatBox;
		chatBox.w = 550;
		chatBox.h = 400;
		chatBox.x = CCFG::GAME_WIDTH - chatBox.w - 20;
		chatBox.y = CCFG::GAME_HEIGHT - chatBox.h - 20;

		SDL_SetRenderDrawBlendMode(rR, SDL_BLENDMODE_BLEND);
		SDL_SetRenderDrawColor(rR, 0, 0, 0, 180);
		SDL_RenderFillRect(rR, &chatBox);

		SDL_SetRenderDrawColor(rR, 255, 255, 255, 255);
		SDL_RenderDrawRect(rR, &chatBox);

		int messagesAreaHeight = chatBox.h - 50;
		int startY = chatBox.y + 10;

		for (const auto& msg : chatMessages) {
			if (startY > chatBox.y + messagesAreaHeight) break;
			CCFG::getText()->Draw(rR, msg, chatBox.x + 10, startY, 10, 255, 255, 255);
			startY += 18;
		}

		SDL_Rect inputBox = {
			chatBox.x + 10,
			chatBox.y + chatBox.h - 35,
			chatBox.w - 20,
			25
		};

		SDL_SetRenderDrawColor(rR, 40, 40, 40, 230);
		SDL_RenderFillRect(rR, &inputBox);

		SDL_SetRenderDrawColor(rR, 255, 255, 255, 255);
		SDL_RenderDrawRect(rR, &inputBox);

		CCFG::getText()->Draw(rR, currentMessage, inputBox.x + 3, inputBox.y + 5, 10, 255, 255, 255);

		SDL_SetRenderDrawBlendMode(rR, SDL_BLENDMODE_NONE);
	}
	
	if (typing) {
		CCFG::getText()->Draw(rR, "-TYPING", 100, 308, 16, 255, 255, 255);
	}
	
}

/* ******************************************** */

void WaitingRoom::enter() {
	switch (activeMenuOption) {
	case 0:
		if (!host) {
			SDL_ShowSimpleMessageBox(SDL_MESSAGEBOX_INFORMATION, "Notice", "Chi co chu phong moi duoc phep bat dau man choi!", nullptr);
			break;
		}

		if (!selectWorld) {
			selectWorld = true;
		}
		else {
			CCFG::getMM()->getLoadingMenu()->updateTime();
			CCore::getMap()->resetGameData();
			CCore::getMap()->setCurrentLevelID(activeWorldID * 4 + activeSecondWorldID);
			CCFG::getMM()->setViewID(CCFG::getMM()->eGameLoading);
			CCFG::getMM()->getLoadingMenu()->loadingType = true;
			CCore::getMap()->setSpawnPointID(0);
			//CCore::getMap()->setMultiplayerMode(false);
			selectWorld = false;
		}
		break;
	case 1:
	{
		if (!selectChat) {
			selectChat = true;
		}
		else {
			runChatThread = true;
			std::thread t([this]() { this->ChatFetchThread(idRoom, CCFG::getUserName(), chatMessages); });
			t.detach();

			typing = true;
			SDL_StartTextInput();
		}
		break;
	}
	case 2:
		CallEscapeRoomAPI(idRoom, CCFG::getUserName());
		runPlayerThread = false;
		if (playerThread.joinable()) playerThread.join();
		CCFG::getMM()->resetActiveOptionID(CCFG::getMM()->eMainMenu);
		CCFG::getMM()->setViewID(CCFG::getMM()->eMainMenu);
		break;

	}
}

void WaitingRoom::escape() {
	if (selectWorld) 
		selectWorld = false;
	if (selectChat) {
		if (typing) {
			typing = false;
			SDL_StopTextInput();
		}
		else {
			selectChat = false;
			runChatThread = false; 
			std::this_thread::sleep_for(std::chrono::milliseconds(100));
		}
	}
}

void WaitingRoom::updateActiveButton(int iDir) {
	if (!host && selectWorld) {
		return;
	}
	
	switch (iDir) {
	case 0: case 2:
		if (!selectWorld) {
			Menu::updateActiveButton(iDir);
		}
		else {
			switch (iDir) {
			case 0:
				if (activeSecondWorldID < 1) {
					activeSecondWorldID = 3;
				}
				else {
					--activeSecondWorldID;
				}
				break;
			case 2:
				if (activeSecondWorldID > 2) {
					activeSecondWorldID = 0;
				}
				else {
					++activeSecondWorldID;
				}
				break;
			}
		}
		break;
	case 1:
		if (selectWorld) {
			if (activeWorldID < 7) {
				++activeWorldID;
			}
			else {
				activeWorldID = 0;
			}
		}
		break;
	case 3:
		if (selectWorld) {
			if (activeWorldID > 0) {
				--activeWorldID;
			}
			else {
				activeWorldID = 7;
			}
		}
		break;
	}
}



void SendMessages(std::string idRoom, std::string name, std::string mess) {
	CURL* curl = curl_easy_init();
	if (!curl) return;

	std::string url = "https://localhost:7244/api/Rooms/sendMessages/" + idRoom + "/" + name;

	std::string jsonBody = "\"" + mess + "\"";

	std::string responseString;

	curl_easy_setopt(curl, CURLOPT_URL, url.c_str());
	curl_easy_setopt(curl, CURLOPT_POSTFIELDS, jsonBody.c_str());
	curl_easy_setopt(curl, CURLOPT_POSTFIELDSIZE, jsonBody.size());

	curl_easy_setopt(curl, CURLOPT_WRITEFUNCTION, WriteCallback);
	curl_easy_setopt(curl, CURLOPT_WRITEDATA, &responseString);

	struct curl_slist* headers = nullptr;
	headers = curl_slist_append(headers, "Content-Type: application/json");
	curl_easy_setopt(curl, CURLOPT_HTTPHEADER, headers);

	curl_easy_setopt(curl, CURLOPT_SSL_VERIFYPEER, 0L);
	curl_easy_setopt(curl, CURLOPT_SSL_VERIFYHOST, 0L);

	CURLcode res = curl_easy_perform(curl);

	curl_slist_free_all(headers);
	curl_easy_cleanup(curl);
}

void WaitingRoom::handleChatInput(const SDL_Event& e) {
	if (!selectChat || !typing) return;

	if (e.type == SDL_TEXTINPUT) {
		std::string s = e.text.text;
		std::transform(s.begin(), s.end(), s.begin(), ::toupper);
		currentMessage += s;
	}
	else if (e.type == SDL_KEYDOWN) {
		if (e.key.keysym.sym == SDLK_BACKSPACE && !currentMessage.empty()) {
			currentMessage.pop_back();
		}
		else if (e.key.keysym.sym == SDLK_RETURN) {
			size_t first = currentMessage.find_first_not_of(" \t\n\r");
			size_t last = currentMessage.find_last_not_of(" \t\n\r");

			if (first != std::string::npos && last != std::string::npos) {
				std::string trimmed = currentMessage.substr(first, last - first + 1);
				if (!trimmed.empty()) {
					SendMessages(idRoom, CCFG::getUserName(), trimmed);
				}
			}

			currentMessage.clear();
		}
		else if (e.key.keysym.sym == SDLK_ESCAPE) {
			typing = false;
			SDL_StopTextInput();
		}
	}
}

void WaitingRoom::PlayerFetchThread(std::string idRoom) {
	while (runPlayerThread) {
		std::string raw = GetPlayersAPI(idRoom);
		if (!raw.empty()) {
			std::vector<std::string> names;

			size_t start = raw.find("[");
			size_t end = raw.find("]");
			if (start != std::string::npos && end != std::string::npos) {
				std::string list = raw.substr(start + 1, end - start - 1);
				std::stringstream ss(list);
				std::string item;
				while (std::getline(ss, item, ',')) {
					item.erase(remove(item.begin(), item.end(), '\"'), item.end());
					item.erase(remove(item.begin(), item.end(), ' '), item.end());
					if (!item.empty()) {
						std::transform(item.begin(), item.end(), item.begin(), ::toupper);
						names.push_back(item);
					}
				}
			}

			{
				std::lock_guard<std::mutex> lock(playerMutex);
				playerNames = names;
			}
		}
		std::this_thread::sleep_for(std::chrono::milliseconds(1000));
	}
}