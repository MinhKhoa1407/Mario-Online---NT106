#pragma once

#pragma once

#ifndef LOGINMENU_H
#define LOGINMENU_H

#include "Menu.h"
#include <string>

class LoginMenu : public Menu
{
private:
	bool loginFailed;
	bool registerFailed;
	bool inLoginForm;
	bool inRegisterForm;
	/*std::string localId;
	std::string idToken;*/
public:
	LoginMenu(void);
	~LoginMenu(void);

	void Update();
	void Draw(SDL_Renderer* rR);

	void enter();
	void escape();

	void updateActiveButton(int iDir);

	/*std::string getLocalId() { return localId; };
	std::string getIdToken() { return idToken; };*/
};

#endif