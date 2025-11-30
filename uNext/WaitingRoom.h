#pragma once

#ifndef WAITINGROOM_H
#define WAITINGROOM_H

#include "Menu.h"
#include <fstream>
#include <algorithm>
#include <cctype>
#include <thread>
#include <mutex>
#include <atomic>
#include <chrono>

class WaitingRoom : public Menu
{
private:
	bool selectWorld, selectChat, typing, host;
	int activeWorldID, activeSecondWorldID;

	std::string currentMessage = "";

	SDL_Rect rSelectWorld;

	Uint32 lastCheck = 0;
	std::vector<std::string> playerNames;
	std::vector<std::string> chatMessages;
	std::string idRoom;

	std::mutex chatMutex;
	std::atomic<bool> runChatThread;

	std::thread playerThread;
	bool runPlayerThread = false;
	std::mutex playerMutex;
public:
	WaitingRoom(void);
	~WaitingRoom(void);

	void Update();
	void Draw(SDL_Renderer* rR);

	void enter();
	void escape();

	void updateActiveButton(int iDir);

	bool isTyping() { return typing; }
	void handleChatInput(const SDL_Event& e);

	void ChatFetchThread(std::string, std::string, std::vector<std::string>&);
	void PlayerFetchThread(std::string);
};

#endif