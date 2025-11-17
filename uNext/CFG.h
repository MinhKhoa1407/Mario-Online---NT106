#pragma once

#ifndef CFG_H
#define CFG_H

#include "Text.h"
#include "MenuManager.h"
#include "Music.h"
#include "Player.h"

class CCFG
{
private:
	static MenuManager* oMM;
	static Text* oText;
	static CIMG* tSMBLOGO;
	static Music* oMusic;
	//static Player* oPlayer;
public:
	CCFG(void);
	~CCFG(void);

	static int GAME_WIDTH, GAME_HEIGHT;

	static bool keySpace;

	static int keyIDA, keyIDS, keyIDD, keyIDSpace, keyIDShift;

	static std::string localId;
	static std::string idToken;

	static std::string getKeyString(int keyID);

	static CIMG* getSMBLOGO();

	static Text* getText();

	static MenuManager* getMM();
	static Music* getMusic();
	//static Player* getScore();

	static bool canMoveBackward;

	static void setLocalId(std::string id) { localId = id; };
	static std::string getLocalId() { return localId; };

	static void setIdToken(std::string token) { idToken = token; };
	static std::string getIdToken() { return idToken; };
};

#endif
