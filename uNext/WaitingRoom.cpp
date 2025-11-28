#include "WaitingRoom.h"
#include "CFG.h"
#include "Core.h"
#include <iostream>
#include <fstream>
#include <vector>
#include <string>
#include <Windows.h> 


/* ******************************************** */

WaitingRoom::WaitingRoom(void) {
	this->lMO.push_back(new MenuOption("PLAY", 40, 276));
	this->lMO.push_back(new MenuOption("CHAT", 40, 308));
	this->lMO.push_back(new MenuOption("ESCAPE", 40, 340));



	this->numOfMenuOptions = lMO.size();

	this->selectWorld = this->selectChat = this->typing = false;

	rSelectWorld.x = 122;
	rSelectWorld.y = 280;
	rSelectWorld.w = 306;
	rSelectWorld.h = 72;

	this->activeWorldID = this->activeSecondWorldID = 0;
	
	playerNames.clear();
	chatMessages.clear();
}

WaitingRoom::~WaitingRoom(void) {

}

/* ******************************************** */

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
	std::string url = "https://localhost:7244/api/Rooms/getMessages/" + idRoom + "?antiPlayerName=" + antiPlayerName;

	

	curl_easy_setopt(curl, CURLOPT_URL, url.c_str());
	/*curl_easy_setopt(curl, CURLOPT_POSTFIELDS, jsonBody.c_str());
	curl_easy_setopt(curl, CURLOPT_POSTFIELDSIZE, jsonBody.size());*/

	curl_easy_setopt(curl, CURLOPT_WRITEFUNCTION, WriteCallback);
	curl_easy_setopt(curl, CURLOPT_WRITEDATA, &responseString);

	/*struct curl_slist* headers = nullptr;
	headers = curl_slist_append(headers, "Content-Type: application/json");
	curl_easy_setopt(curl, CURLOPT_HTTPHEADER, headers);*/

	// Bật tắt SSL nếu chạy localhost
	curl_easy_setopt(curl, CURLOPT_SSL_VERIFYPEER, 0L);
	curl_easy_setopt(curl, CURLOPT_SSL_VERIFYHOST, 0L);

	CURLcode res = curl_easy_perform(curl);

	// Debug (bật khi cần)
	// if (res == CURLE_OK)
	//     std::cout << "Response: " << responseString << std::endl;
	// else
	//     std::cout << "Error: " << curl_easy_strerror(res) << std::endl;

	//CURLcode res = curl_easy_perform(curl);

	curl_easy_cleanup(curl);
	return responseString;

	/*curl_slist_free_all(headers);
	curl_easy_cleanup(curl);*/
}

void WaitingRoom::ChatFetchThread(std::string idRoom, std::string userName, std::vector<std::string>& chatMessages) {
	while (runChatThread) {
		std::string mess = GetMessagesFromAPI(idRoom, userName);
		if (!mess.empty() && mess != "NULL") {
			std::lock_guard<std::mutex> lock(chatMutex);
			if (std::find(chatMessages.begin(), chatMessages.end(), mess) == chatMessages.end()) {
				chatMessages.push_back(mess);
			}
		}
		std::this_thread::sleep_for(std::chrono::milliseconds(200));
	}
}

void WaitingRoom::Update() {
	Menu::Update();

	Uint32 now = SDL_GetTicks();
	if (now - lastCheck >= 1000) {
		lastCheck = now;

		std::ifstream file("room_info.txt");
		if (file.is_open()) {
			playerNames.clear();
			std::string line;

			int i = 0;
			while (std::getline(file, line)) {
				if (!line.empty()) {
					if (i == 0) {
						idRoom = line;
						i = 1;
						continue;
					}
					std::transform(line.begin(), line.end(), line.begin(), ::toupper);
					playerNames.push_back(line);
				}
			}

			file.close();
		}

		/*std::string mess = GetMessagesFromAPI(idRoom, CCFG::getUserName());
		if (mess != "NULL" && std::find(chatMessages.begin(), chatMessages.end(), mess) == chatMessages.end()) {
			chatMessages.push_back(mess);
		}*/
	}
}

void WaitingRoom::Draw(SDL_Renderer* rR) {
	SDL_Rect playerBox;
	playerBox.x = 20;
	playerBox.y = 80;
	playerBox.w = 200;
	playerBox.h = 180;

	// nền mờ
	SDL_SetRenderDrawBlendMode(rR, SDL_BLENDMODE_BLEND);
	SDL_SetRenderDrawColor(rR, 0, 0, 0, 170);   // đen mờ
	SDL_RenderFillRect(rR, &playerBox);

	// viền trắng
	SDL_SetRenderDrawColor(rR, 255, 255, 255, 255);
	SDL_RenderDrawRect(rR, &playerBox);

	// reset blend mode
	SDL_SetRenderDrawBlendMode(rR, SDL_BLENDMODE_NONE);

	Menu::Draw(rR);
	//CCFG::getText()->Draw(rR, "WWW.LUKASZJAKOWSKI.PL", 4, CCFG::GAME_HEIGHT - 4 - 8, 8, 0, 0, 0);
	//CCFG::getText()->Draw(rR, "WWW.LUKASZJAKOWSKI.PL", 5, CCFG::GAME_HEIGHT - 5 - 8, 8, 255, 255, 255);

	CCFG::getText()->Draw(rR, "MEMBERS:", 50, 100, 18, 255, 255, 255);
	int startY = 150;
	for (const auto& name : playerNames) {
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

		// --- Vùng hiển thị tin nhắn ---
		int messagesAreaHeight = chatBox.h - 50;
		int startY = chatBox.y + 10;

		for (const auto& msg : chatMessages) {
			if (startY > chatBox.y + messagesAreaHeight) break;
			CCFG::getText()->Draw(rR, msg, chatBox.x + 10, startY, 12, 255, 255, 255);
			startY += 18;
		}

		// --- Ô nhập tin nhắn ---
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

		// **LUÔN HIỂN THỊ currentMessage**
		CCFG::getText()->Draw(rR, currentMessage, inputBox.x + 5, inputBox.y + 5, 12, 255, 255, 255);

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
			runChatThread = false; // thread sẽ tự thoát vòng lặp
			std::this_thread::sleep_for(std::chrono::milliseconds(100));
		}
	}
}

void WaitingRoom::updateActiveButton(int iDir) {
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

	// URL gồm Id và playerName
	std::string url = "https://localhost:7244/api/Rooms/sendMessages/" + idRoom + "/" + name;

	// Body phải là JSON dạng string
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

	// Bật tắt SSL nếu chạy localhost
	curl_easy_setopt(curl, CURLOPT_SSL_VERIFYPEER, 0L);
	curl_easy_setopt(curl, CURLOPT_SSL_VERIFYHOST, 0L);

	CURLcode res = curl_easy_perform(curl);

	// Debug (bật khi cần)
	// if (res == CURLE_OK)
	//     std::cout << "Response: " << responseString << std::endl;
	// else
	//     std::cout << "Error: " << curl_easy_strerror(res) << std::endl;

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
					chatMessages.push_back(trimmed);
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