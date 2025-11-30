#pragma once

#ifndef ABOUTMENU_H
#define ABOUTMENU_H

#include "Menu.h"
#include <iostream>
#include <string>
#include <curl/curl.h>
#include "APIManager.h"

class AboutMenu : public Menu
{
private:
	unsigned int iTime;
	APIManager api;
	int cR, cG, cB, nR, nG, nB;
	int colorStepID, iColorID;

	// ----- true = RIGHT, false = LEFT
	bool moveDirection;

	int iNumOfUnits;
public:
	AboutMenu(void);
	~AboutMenu(void);

	void Update();
	void Draw(SDL_Renderer* rR);

	void enter();
	void escape();
	void launch();
	void reset();

	void nextColor();
	int getColorStep(int iOld, int iNew);

	void setBackgroundColor(SDL_Renderer* rR);
	void updateTime();

	void updateActiveButton(int iDir);
};

#endif