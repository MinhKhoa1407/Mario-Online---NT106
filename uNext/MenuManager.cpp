#include "MenuManager.h"
#include "Core.h"

/* ******************************************** */

MenuManager::MenuManager(void) {
	this->currentGameState = eLoginMenu;
	//this->currentGameState = eMainMenu;
	this->oLoginMenu = new LoginMenu();
	this->oMainMenu = new MainMenu();
	this->oLoadingMenu = new LoadingMenu();
	this->oAboutMenu = new AboutMenu();
	//this->oConsole = new Console();
	this->oOptionsMenu = new OptionsMenu();
	//this->oLE = new LevelEditor();
	this->oPauseMenu = new PauseMenu();
	this->oMultiplayerMenu = new MultiplayerMenu();
	this->oWaitingRoom = new WaitingRoom();
}


MenuManager::~MenuManager(void) {
	delete activeOption;
	delete oLoginMenu;
	delete oMainMenu;
	delete oLoadingMenu;
	delete oAboutMenu;
	delete oPauseMenu;
	delete oMultiplayerMenu;
	delete oWaitingRoom;
}

/* ******************************************** */

void MenuManager::Update() {
	switch (currentGameState) {
	case eLoginMenu:
		oLoginMenu->Update();
		break;
	case eMainMenu:
		oMainMenu->Update();
		CCore::getMap()->UpdateBlocks();
		break;
	case eGameLoading:
		oLoadingMenu->Update();
		break;
	case eGame:
		CCore::getMap()->Update();
		CCore::getMap()->UpdateMinionsCollisions();
		//oLE->Update();
		break;
	case eAbout:
		CCore::getMap()->UpdateMinions();
		CCore::getMap()->UpdateMinionBlokcs();
		CCore::getMap()->UpdateBlocks();
		oAboutMenu->Update();
		break;
	case eOptions:
		oOptionsMenu->Update();
		break;
	case ePause:
		oPauseMenu->Update();
		break;
	case eMultiplayerMenu:
		CCore::getMap()->UpdateBlocks();
		oMultiplayerMenu->Update();
		break;
	case eWaitingRoom:
		oWaitingRoom->Update();
		CCore::getMap()->UpdateBlocks();
		break;
	}
}

void MenuManager::Draw(SDL_Renderer* rR) {
	switch (currentGameState) {
	case eLoginMenu:
		CCore::getMap()->DrawMap(rR);
		oLoginMenu->Draw(rR);
		break;
	case eMainMenu:
		CCore::getMap()->DrawMap(rR);
		CCore::getMap()->getPlayer()->Draw(rR);
		CCore::getMap()->DrawGameLayout(rR);
		oMainMenu->Draw(rR);
		break;
	case eGameLoading:
		oLoadingMenu->Draw(rR);
		break;
	case eGame:
		CCore::getMap()->Draw(rR);
		//oLE->Draw(rR);
		break;
	case eAbout:
		CCore::getMap()->DrawMap(rR);
		//CCore::getMap()->getPlayer()->Draw(rR);
		CCore::getMap()->DrawMinions(rR);
		oAboutMenu->Draw(rR);
		break;
	case eOptions:
		CCore::getMap()->DrawMap(rR);
		CCore::getMap()->DrawMinions(rR);
		CCore::getMap()->getPlayer()->Draw(rR);
		CCore::getMap()->DrawGameLayout(rR);
		oOptionsMenu->Draw(rR);
		break;
	case ePause:
		CCore::getMap()->DrawMap(rR);
		CCore::getMap()->DrawMinions(rR);
		CCore::getMap()->getPlayer()->Draw(rR);
		CCore::getMap()->DrawGameLayout(rR);
		oPauseMenu->Draw(rR);
		break;
	case eMultiplayerMenu:
		// Vẽ nền Map phía sau cho đẹp (giống Main Menu)
		CCore::getMap()->DrawMap(rR);
		CCore::getMap()->getPlayer()->Draw(rR);
		// Vẽ giao diện Multiplayer đè lên trên
		oMultiplayerMenu->Draw(rR);
		break;
	case eWaitingRoom:
		CCore::getMap()->DrawMap(rR);
		oWaitingRoom->Draw(rR);
		break;
	}
	

	/* -- CRT EFFECT
	SDL_SetRenderDrawBlendMode(rR, SDL_BLENDMODE_BLEND);
	SDL_SetRenderDrawColor(rR, 0, 0, 0, CCFG::getMusic()->getVolume());
	for(int i = 0; i < CCFG::GAME_WIDTH; i += 2) {
		SDL_RenderDrawLine(rR, i, 0, i, CCFG::GAME_WIDTH);
	}
	for(int i = 0; i < CCFG::GAME_HEIGHT; i += 2) {
		SDL_RenderDrawLine(rR, 0, i, CCFG::GAME_WIDTH, i);
	}*/

	//oConsole->Draw(rR);
}

void MenuManager::setBackgroundColor(SDL_Renderer* rR) {
	switch (currentGameState) {
	case eLoginMenu:
		CCore::getMap()->setBackgroundColor(rR);
		break;
	case eMainMenu:
		CCore::getMap()->setBackgroundColor(rR);
		break;
	case eGameLoading:
		SDL_SetRenderDrawColor(rR, 0, 0, 0, 255);
		break;
	case eGame:
		CCore::getMap()->setBackgroundColor(rR);
		break;
	case eAbout:
		oAboutMenu->setBackgroundColor(rR);
		break;
	case eWaitingRoom:
		CCore::getMap()->setBackgroundColor(rR);
		break;
	}

}

/* ******************************************** */

void MenuManager::enter() {
	switch (currentGameState) {
	case eLoginMenu:
		oLoginMenu->enter();
		break;
	case eMainMenu:
		oMainMenu->enter();
		break;
	case eGame:
		CCore::getMap()->setDrawLines(!CCore::getMap()->getDrawLines());
		break;
	case eAbout:
		oAboutMenu->enter();
		break;
	case eOptions:
		oOptionsMenu->enter();
		break;
	case ePause:
		oPauseMenu->enter();
		break;
	case eMultiplayerMenu:
		oMultiplayerMenu->enter();
		break;
	case eWaitingRoom:
		oWaitingRoom->enter();
		break;
	}
}

void MenuManager::escape() {
	switch (currentGameState) {
	case eGame:
		break;
	case eAbout:
		oAboutMenu->enter();
		break;
	case eOptions:
		oOptionsMenu->escape();
		break;
	case ePause:
		oPauseMenu->escape();
		break;
	case eMainMenu:
		oMainMenu->escape();
		break;
	case eMultiplayerMenu:
		oMultiplayerMenu->escape();
		break;
	case eWaitingRoom:
		oWaitingRoom->escape();
		break;
	}
}

void MenuManager::setKey(int keyID) {
	switch (currentGameState) {
	case eOptions:
		oOptionsMenu->setKey(keyID);
		break;
	}
}

void MenuManager::keyPressed(int iDir) {
	switch (currentGameState) {
	case eLoginMenu:
		oLoginMenu->updateActiveButton(iDir);
		break;
	case eMainMenu:
		oMainMenu->updateActiveButton(iDir);
		break;
	case eOptions:
		oOptionsMenu->updateActiveButton(iDir);
		break;
	case eAbout:
		oAboutMenu->updateActiveButton(iDir);
		break;
	case ePause:
		oPauseMenu->updateActiveButton(iDir);
		break;
	case eMultiplayerMenu:
		oMultiplayerMenu->updateActiveButton(iDir);
		break;
	case eWaitingRoom:
		oWaitingRoom->updateActiveButton(iDir);
		break;
	}
}

void MenuManager::resetActiveOptionID(gameState ID) {
	switch (ID) {
	case eMainMenu:
		oMainMenu->activeMenuOption = 0;
		break;
	case eOptions:
		oOptionsMenu->activeMenuOption = 0;
		break;
	case ePause:
		oPauseMenu->activeMenuOption = 0;
		break;
	case eMultiplayerMenu:
		oMultiplayerMenu->activeMenuOption = 0;
		break;
	case eWaitingRoom:
		oWaitingRoom->activeMenuOption = 0;
		break;
	}
}

/* ******************************************** */

int MenuManager::getViewID() {
	return currentGameState;
}

void MenuManager::setViewID(gameState viewID) {
	this->currentGameState = viewID;
}

CIMG* MenuManager::getActiveOption() {
	return activeOption;
}

void MenuManager::setActiveOption(SDL_Renderer* rR) {
	activeOption = new CIMG("active_option", rR);
}

LoadingMenu* MenuManager::getLoadingMenu() {
	return oLoadingMenu;
}

AboutMenu* MenuManager::getAboutMenu() {
	return oAboutMenu;
}
/*
Console* MenuManager::getConsole() {
	return oConsole;
}

LevelEditor* MenuManager::getLE() {
	return oLE;
}
*/
OptionsMenu* MenuManager::getOptions() {
	return oOptionsMenu;
}

LoginMenu* MenuManager::getLoginMenu() {
	return oLoginMenu;
}

MultiplayerMenu* MenuManager::getMultiplayerMenu() {
	return oMultiplayerMenu;
}

WaitingRoom* MenuManager::getWaitingRoom() {
	return oWaitingRoom;
}
