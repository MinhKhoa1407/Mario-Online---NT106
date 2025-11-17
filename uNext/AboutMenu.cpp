#include "AboutMenu.h"
#include "CFG.h"
#include "Core.h"
#include "stdlib.h"
#include "time.h"
#include <iostream>
#include <fstream>

/* ******************************************** */

AboutMenu::AboutMenu(void) {
	this->lMO.push_back(new MenuOption("RANKING", 150, 220));  
	this->lMO.push_back(new MenuOption("LOG OUT", 150, 260));  
	this->lMO.push_back(new MenuOption("MAIN MENU", 150, 340)); 

	this->numOfMenuOptions = lMO.size();

	this->cR = 93;
	this->cG = 148;
	this->cB = 252;
	this->nR = 0;
	this->nG = 0;
	this->nB = 0;
	this->colorStepID = 1;
	this->iColorID = 0;

	this->moveDirection = true;

	this->iNumOfUnits = 0;

	srand((unsigned)time(NULL));
}


AboutMenu::~AboutMenu(void) {

}

/* ******************************************** */

void AboutMenu::Update() {
	if(SDL_GetTicks() >= iTime + 35) {
		this->cR = getColorStep(cR, nR);
		this->cG = getColorStep(cG, nG);
		this->cB = getColorStep(cB, nB);

		if (colorStepID >= 15 || (cR == nR && cG == nG && cB == nB)) {
			nextColor();
			colorStepID = 1;
		} else {
			++colorStepID;
		}

		CCore::getMap()->setLevelType(rand()%4);
		
		if(rand()%10 < 6) {
			CCore::getMap()->addGoombas(-(int)CCore::getMap()->getXPos() + rand() % (CCFG::GAME_WIDTH + 128), -32, rand()%2 == 0);
			CCore::getMap()->addGoombas(-(int)CCore::getMap()->getXPos() + rand() % (CCFG::GAME_WIDTH + 128), -32, rand()%2 == 0);
		} else if(rand()%10 < 8) {
			CCore::getMap()->addKoppa(-(int)CCore::getMap()->getXPos() + rand() % (CCFG::GAME_WIDTH + 128), -32, 0, rand()%2 == 0);
			CCore::getMap()->addKoppa(-(int)CCore::getMap()->getXPos() + rand() % (CCFG::GAME_WIDTH + 128), -32, 0, rand()%2 == 0);
		} else if(rand()%6 < 4) {
			CCore::getMap()->addFire(-CCore::getMap()->getXPos() + CCFG::GAME_WIDTH + 128, CCFG::GAME_HEIGHT - 16.0f - rand()%16*32, CCFG::GAME_HEIGHT - 16 - rand()%16*32);
		} else if(rand()%6 < 4) {
			CCore::getMap()->addBulletBill((int)(-CCore::getMap()->getXPos() + CCFG::GAME_WIDTH + 128), CCFG::GAME_HEIGHT - 16 - rand()%16*32, true, 1);
		} else {
			CCore::getMap()->addFireBall(-(int)CCore::getMap()->getXPos() + rand() % (CCFG::GAME_WIDTH + 128) + 8, CCFG::GAME_HEIGHT - 16 - rand()%16 * 32, rand()%8 + 4 + 8, rand()%360, rand()%2 == 0);
		}

		iNumOfUnits += 2;

		iTime = SDL_GetTicks();
	}
	
	if(moveDirection && CCFG::GAME_WIDTH - CCore::getMap()->getXPos() >= (CCore::getMap()->getMapWidth() - 20) * 32) {
		moveDirection = !moveDirection;
	} else if(!moveDirection && -CCore::getMap()->getXPos() <= 0) {
		moveDirection = !moveDirection;
	}

	CCore::getMap()->setXPos(CCore::getMap()->getXPos() + 4 * (moveDirection ? -1 : 1));
	CCore::getMap()->getPlayer()->setXPos((float)CCore::getMap()->getPlayer()->getXPos() + 4 * (moveDirection ? -1 : 1));
	Menu::Update();
}

void AboutMenu::Draw(SDL_Renderer* rR) {
	/*CCFG::getText()->DrawWS(rR, "MARIO V 1.03 - C++ AND SDL2", 150, 128, 0, 0, 0);
	CCFG::getText()->DrawWS(rR, "AUTOR: LUKASZ JAKOWSKI", 150, 146, 0, 0, 0);

	CCFG::getText()->DrawWS(rR, "INFORMATYKA INZ 2012-2016", 150, 188, 0, 0, 0);
	CCFG::getText()->DrawWS(rR, "UNIWERSYTET SLASKI W KATOWICACH", 150, 206, 0, 0, 0);
	CCFG::getText()->DrawWS(rR, "MAJ 2014", 150, 224, 0, 0, 0);

	CCFG::getText()->DrawWS(rR, "WWW.LUKASZJAKOWSKI.PL", 150, 264, 0, 0, 0);*/

	Menu::Draw(rR);

	//CCFG::getText()->DrawWS(rR, std::to_string(iNumOfUnits), 5, CCFG::GAME_HEIGHT - 21, 0, 0, 0);

	/*for(unsigned int i = 0; i < lMO.size(); i++) {
		CCFG::getText()->DrawWS(rR, lMO[i]->getText(), lMO[i]->getXPos(), lMO[i]->getYPos(), 0, 0, 0);

	}

	CCFG::getMM()->getActiveOption()->Draw(rR, lMO[activeMenuOption]->getXPos() - 32, lMO[activeMenuOption]->getYPos());*/
}

/* ******************************************** */

static size_t WriteCallback(void* contents, size_t size, size_t nmemb, void* userp)
{
	((std::string*)userp)->append((char*)contents, size * nmemb);
	return size * nmemb;
}

void CallLogoutAPI(const std::string& localId, const std::string& idToken) {
	CURL* curl = curl_easy_init();
	if (!curl) {
		/*std::cerr << "Failed to init curl" << std::endl;*/
		return;
	}

	std::string url = "https://localhost:7244/api/Firebase/logout";
	std::string jsonData = "{\"localId\":\"" + localId + "\",\"idToken\":\"" + idToken + "\"}";
	std::string responseString;

	curl_easy_setopt(curl, CURLOPT_URL, url.c_str());
	curl_easy_setopt(curl, CURLOPT_POSTFIELDS, jsonData.c_str());
	curl_easy_setopt(curl, CURLOPT_WRITEFUNCTION, WriteCallback);
	curl_easy_setopt(curl, CURLOPT_WRITEDATA, &responseString);

	struct curl_slist* headers = nullptr;
	headers = curl_slist_append(headers, "Content-Type: application/json");
	curl_easy_setopt(curl, CURLOPT_HTTPHEADER, headers);

	curl_easy_setopt(curl, CURLOPT_SSL_VERIFYPEER, 0L);
	curl_easy_setopt(curl, CURLOPT_SSL_VERIFYHOST, 0L);

	CURLcode res = curl_easy_perform(curl);
	/*if (res != CURLE_OK) {
		std::cerr << "Curl failed: " << curl_easy_strerror(res) << std::endl;
	}
	else {
		std::cout << "Request sent successfully. Response: " << responseString << std::endl;
	}*/

	curl_slist_free_all(headers);
	curl_easy_cleanup(curl);
}

void RankingForm() {
	char exePath[MAX_PATH];
	GetModuleFileNameA(NULL, exePath, MAX_PATH);

	std::string currentDir = exePath;
	size_t pos = currentDir.find_last_of("\\/");
	currentDir = currentDir.substr(0, pos);

	std::string projectRoot = currentDir.substr(0, currentDir.find_last_of("\\/"));
	projectRoot = projectRoot.substr(0, projectRoot.find_last_of("\\/"));

	std::string rankingExe = projectRoot + "\\Ranking\\bin\\Debug\\Ranking.exe";

	std::string cmdLine = "\"" + rankingExe + "\" " + CCFG::getLocalId() + " " + CCFG::getIdToken();
	std::vector<char> cmdLineBuffer(cmdLine.begin(), cmdLine.end());
	cmdLineBuffer.push_back('\0');

	STARTUPINFOA si = { sizeof(si) };
	PROCESS_INFORMATION pi;

	if(CreateProcessA(
		rankingExe.c_str(),        
		cmdLineBuffer.data(),      
		NULL, NULL, FALSE,
		0,
		NULL, NULL,
		&si, &pi
	))
	{
		WaitForSingleObject(pi.hProcess, INFINITE);

		CloseHandle(pi.hProcess);
		CloseHandle(pi.hThread);
	}
}

void AboutMenu::enter() {
	switch (activeMenuOption) {
		case 0:
			RankingForm();
			break;
		case 1: {
			std::string localId = CCFG::getLocalId();
			std::string idToken = CCFG::getIdToken();
			if (localId == "" || idToken == "") {
				CCFG::getMM()->resetActiveOptionID(CCFG::getMM()->eMainMenu);
				break;
			}
			CallLogoutAPI(localId, idToken);

			CCFG::getMM()->resetActiveOptionID(CCFG::getMM()->eLoginMenu);
			CCFG::getMM()->setViewID(CCFG::getMM()->eLoginMenu);
			reset();
			CCFG::getMusic()->StopMusic();
			break;
		}
		case 2:
			CCFG::getMM()->resetActiveOptionID(CCFG::getMM()->eMainMenu);
			CCFG::getMM()->setViewID(CCFG::getMM()->eMainMenu);
			reset();
			CCFG::getMusic()->StopMusic();
			break;
	}
}

/* ******************************************** */

void AboutMenu::launch() {
	this->cR = 93;
	this->cG = 148;
	this->cB = 252;
}

void AboutMenu::reset() {
	CCore::getMap()->setXPos(0);
	CCore::getMap()->loadLVL();
}

/* ******************************************** */

void AboutMenu::nextColor() {
	int iN = iColorID;

	while(iN == iColorID) {
		iColorID = rand() % 16;
	}

	++iColorID;

	switch (iColorID) {
		case 0:
			nR = 73;
			nG = 133;
			nB = 203;
			break;
		case 1:
			nR = 197;
			nG = 197;
			nB = 223;
			break;
		case 2:
			nR = 27;
			nG = 60;
			nB = 173;
			break;
		case 3:
			nR = 6;
			nG = 21;
			nB = 86;
			break;
		case 4:
			nR = 183;
			nG = 85;
			nB = 76;
			break;
		case 5:
			nR = 110;
			nG = 58;
			nB = 70;
			break;
		case 6:
			nR = 55;
			nG = 19;
			nB = 63;
			break;
		case 7:
			nR = 115;
			nG = 53;
			nB = 126;
			break;
		case 8:
			nR = 227;
			nG = 200;
			nB = 0;
			break;
		case 9:
			nR = 255;
			nG = 180;
			nB = 2;
			break;
		case 10:
			nR = 231;
			nG = 51;
			nB = 24;
			break;
		case 11:
			nR = 255;
			nG = 180;
			nB = 2;
			break;
		case 12:
			nR = 4;
			nG = 2;
			nB = 15;
			break;
		case 13:
			nR = 135;
			nG = 178;
			nB = 168;
			break;
		case 14:
			nR = 64;
			nG = 43;
			nB = 24;
			break;
		case 15:
			nR = rand() % 255;
			nG = rand() % 255;
			nB = rand() % 255;
			break;
	}
}

int AboutMenu::getColorStep(int iOld, int iNew) {
	return iOld + (iOld > iNew ? (iNew - iOld) * colorStepID / 30 : (iNew - iOld) * colorStepID / 30);
}

/* ******************************************** */

void AboutMenu::setBackgroundColor(SDL_Renderer* rR) {
	SDL_SetRenderDrawColor(rR, cR, cG, cB, 255);
}

void AboutMenu::updateTime() {
	this->iTime = SDL_GetTicks();
}

void AboutMenu::updateActiveButton(int iDir) {
	switch (iDir) {
	case 0: case 2:
		Menu::updateActiveButton(iDir);
		break;
	}
}