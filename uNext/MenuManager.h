#pragma once

#ifndef MENUMANAGER_H
#define MENUMANAGER_H

#include "IMG.h"
#include "MainMenu.h"
#include "LoadingMenu.h"
#include "AboutMenu.h"
#include "Console.h"
#include "LevelEditor.h"
#include "OptionsMenu.h"
#include "PauseMenu.h"
#include "LoginMenu.h"
#include "MultiplayerMenu.h"
#include "WaitingRoom.h"

class MenuManager
{
private:
	CIMG* activeOption;

	LoginMenu* oLoginMenu;
	MainMenu* oMainMenu;
	LoadingMenu* oLoadingMenu;
	AboutMenu* oAboutMenu;
	OptionsMenu* oOptionsMenu;
	PauseMenu* oPauseMenu;
	MultiplayerMenu* oMultiplayerMenu;
	WaitingRoom* oWaitingRoom;
	//Console* oConsole;
	//LevelEditor* oLE;

public:
	MenuManager(void);
	~MenuManager(void);

	enum gameState {
		eLoginMenu,
		eMainMenu,
		eGameLoading,
		eGame,
		eAbout,
		eOptions,
		ePause,
		eMultiplayerMenu,
		eWaitingRoom
		//eLevelEditor,
	};

	gameState currentGameState;

	void Update();
	void Draw(SDL_Renderer* rR);

	void setBackgroundColor(SDL_Renderer* rR);

	void enter();
	void escape();
	void setKey(int keyID);
	void keyPressed(int iDir);

	void resetActiveOptionID(gameState ID);

	int getViewID();
	void setViewID(gameState viewID);

	CIMG* getActiveOption();
	void setActiveOption(SDL_Renderer* rR);

	LoginMenu* getLoginMenu();

	LoadingMenu* getLoadingMenu();
	AboutMenu* getAboutMenu();

	//Console* getConsole();
	//LevelEditor* getLE();
	OptionsMenu* getOptions();
	MultiplayerMenu* getMultiplayerMenu();
	WaitingRoom* getWaitingRoom();
};

#endif