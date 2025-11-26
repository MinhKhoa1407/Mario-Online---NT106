#include "MultiplayerMenu.h"
#include "CFG.h"
#include "Core.h"
#include <iostream>
#include <string>
#include <Windows.h>

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
void MultiplayerMenu::OpenLauncher(std::string mode) {
    // Xóa kết quả cũ
    remove("room_info.txt");

    // Thêm tham số vào dòng lệnh: "RoomLauncher.exe create"
    std::string cmd = "RoomLauncher.exe " + mode;

    STARTUPINFOA si = { sizeof(si) };
    PROCESS_INFORMATION pi;

    if (CreateProcessA(NULL, (LPSTR)cmd.c_str(), NULL, NULL, FALSE, 0, NULL, NULL, &si, &pi))
    {
        WaitForSingleObject(pi.hProcess, INFINITE);
        CloseHandle(pi.hProcess);
        CloseHandle(pi.hThread);

        // Đọc kết quả (Logic giống hệt MainMenu cũ)
        // ... (Bạn có thể copy đoạn đọc file room_info.txt vào đây để xử lý vào game luôn)
    }
}

void MultiplayerMenu::enter() {
    switch (activeMenuOption) {
    case 0: // CREATE
        OpenLauncher("create");
        break;
    case 1: // JOIN
        OpenLauncher("join");
        break;
    case 2: // BACK
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