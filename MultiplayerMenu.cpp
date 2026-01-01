#include "MultiplayerMenu.h"
#include "CFG.h"
#include "Core.h"
#include <iostream>
#include <string>
#include <Windows.h>
#include <fstream>

MultiplayerMenu::MultiplayerMenu(void) {
    // Tạo 2 nút chọn
    this->lMO.push_back(new MenuOption("CREATE NEW ROOM", 160, 240));
    this->lMO.push_back(new MenuOption("JOIN EXISTING ROOM", 160, 272));
    this->lMO.push_back(new MenuOption("BACK", 160, 320));

    this->numOfMenuOptions = lMO.size();
}

MultiplayerMenu::~MultiplayerMenu(void) {}

void MultiplayerMenu::Update() {
    Menu::Update();
}

void MultiplayerMenu::Draw(SDL_Renderer* rR) {
    // Vẽ nền mờ
    SDL_SetRenderDrawBlendMode(rR, SDL_BLENDMODE_BLEND);
    SDL_SetRenderDrawColor(rR, 0, 0, 0, 180);
    SDL_RenderFillRect(rR, NULL);
    SDL_SetRenderDrawBlendMode(rR, SDL_BLENDMODE_NONE);

    CCFG::getText()->Draw(rR, "MULTIPLAYER MODE", CCFG::GAME_WIDTH / 2 - CCFG::getText()->getTextWidth("MULTIPLAYER MODE") / 2, 100, 16, 255, 255, 255);

    Menu::Draw(rR);
}

// Hàm mở Form C# với tham số
void MultiplayerMenu::OpenLauncher(std::string mode, std::string username) {
    remove("room_info.txt");

    char exePath[MAX_PATH];
    GetModuleFileNameA(NULL, exePath, MAX_PATH);

    std::string currentDir = exePath;
    size_t pos = currentDir.find_last_of("\\/");
    currentDir = currentDir.substr(0, pos);

    std::string projectRoot = currentDir.substr(0, currentDir.find_last_of("\\/"));
    projectRoot = projectRoot.substr(0, projectRoot.find_last_of("\\/"));

    std::string launcherExe =
        projectRoot + "\\RoomLauncher\\bin\\Debug\\net8.0-windows\\RoomLauncher.exe";

    // Command line: RoomLauncher.exe <mode>
    std::string cmdLine = "\"" + launcherExe + "\" " + mode + " " + username;

    // Convert sang buffer
    std::vector<char> cmdLineBuffer(cmdLine.begin(), cmdLine.end());
    cmdLineBuffer.push_back('\0');

    STARTUPINFOA si = { sizeof(si) };
    PROCESS_INFORMATION pi;

    if (CreateProcessA(
        launcherExe.c_str(),
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

        // Đọc file room_info.txt nếu cần
        // ...
    }
}

void MultiplayerMenu::enter() {
    std::string username = CCFG::getUserName();
    switch (activeMenuOption) {
    case 0: {
        OpenLauncher("create", username);
        std::ifstream file("room_info.txt");
        if (!file.is_open()) break;

        /*std::string result;
        std::getline(file, result);*/

        file.close();
        CCFG::getMM()->getWaitingRoom()->ResetState();
        CCFG::getMM()->resetActiveOptionID(CCFG::getMM()->eWaitingRoom);
        CCFG::getMM()->setViewID(CCFG::getMM()->eWaitingRoom);
        break;
    }
    case 1: { // JOIN
        OpenLauncher("join", username);
        std::ifstream file("room_info.txt");
        if (!file.is_open()) break;

        /*std::string result;
        std::getline(file, result);*/

        file.close();
        CCFG::getMM()->getWaitingRoom()->ResetState();
        CCFG::getMM()->resetActiveOptionID(CCFG::getMM()->eWaitingRoom);
        CCFG::getMM()->setViewID(CCFG::getMM()->eWaitingRoom);
        break;
    }
    case 2: // BACK
        CCFG::getMM()->resetActiveOptionID(CCFG::getMM()->eMainMenu);
        CCFG::getMM()->setViewID(CCFG::getMM()->eMainMenu);
        break;
    }
}

void MultiplayerMenu::escape() {
    CCFG::getMM()->setViewID(CCFG::getMM()->eMainMenu);
}

void MultiplayerMenu::updateActiveButton(int iDir) {
    Menu::updateActiveButton(iDir);
}