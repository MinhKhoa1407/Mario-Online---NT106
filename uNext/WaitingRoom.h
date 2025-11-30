#pragma once

#ifndef WAITINGROOM_H
#define WAITINGROOM_H

#include "Menu.h"

class WaitingRoom : public Menu
{
private:
	bool selectWorld;
	int activeWorldID, activeSecondWorldID;

	SDL_Rect rSelectWorld;
public:
	WaitingRoom(void);
	~WaitingRoom(void);

	void Update();
	void Draw(SDL_Renderer* rR);

	void enter();
	void escape();

	void updateActiveButton(int iDir);
};

#endif