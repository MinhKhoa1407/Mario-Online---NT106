#include "LoginMenu.h"
#include "Core.h"
#include "CFG.h"
#include <Windows.h>
#include <iostream>
#include <fstream>

LoginMenu::LoginMenu(void) {
    this->lMO.push_back(new MenuOption("LOGIN", 230, 300));
    this->lMO.push_back(new MenuOption("REGISTER", 230, 330));

    this->numOfMenuOptions = lMO.size();

    loginFailed = false;
    registerFailed = false;
    inLoginForm = false;
    inRegisterForm = false;

}

LoginMenu::~LoginMenu(void) {
}

void LoginMenu::Update() {
    Menu::Update();
}

void LoginMenu::Draw(SDL_Renderer* rR) {

    CCFG::getSMBLOGO()->Draw(rR, 80, 48);
    Menu::Draw(rR);
    CCFG::getText()->Draw(rR, "WWW.LUKASZJAKOWSKI.PL", 4, CCFG::GAME_HEIGHT - 4 - 8, 8, 0, 0, 0);
    CCFG::getText()->Draw(rR, "WWW.LUKASZJAKOWSKI.PL", 5, CCFG::GAME_HEIGHT - 5 - 8, 8, 255, 255, 255);
    if (loginFailed) {
        CCFG::getText()->Draw(rR, "LOGIN FAILED!", 150, 270, 16, 255, 0, 0);
    }
    else if (registerFailed) {
        CCFG::getText()->Draw(rR, "REGISTER FAILED!", 150, 270, 16, 255, 0, 0);
    }
    /*else if (inLoginForm || inRegisterForm) {
        CCFG::getText()->Draw(rR, "LOADING...", 150, 270, 16, 255, 0, 0);
    }*/
    else {
        CCFG::getText()->Draw(rR, " ", 150, 270, 16, 255, 0, 0);
    }
}

std::string runLoginFormAndReadResult(std::string& id, std::string& token, std::string& name) {
    remove("login_info.txt");
    
    char exePath[MAX_PATH];
    GetModuleFileNameA(NULL, exePath, MAX_PATH);
    std::string status;

    std::string currentDir = exePath;
    size_t pos = currentDir.find_last_of("\\/");
    if (pos != std::string::npos)
        currentDir = currentDir.substr(0, pos);

    std::string projectRoot = currentDir.substr(0, currentDir.find_last_of("\\/"));
    projectRoot = projectRoot.substr(0, projectRoot.find_last_of("\\/"));

    std::string loginExe = projectRoot + "\\Login\\bin\\Debug\\Login.exe";

    std::string cmdLine = "\"" + loginExe + "\"";
    std::vector<char> cmdLineBuffer(cmdLine.begin(), cmdLine.end());
    cmdLineBuffer.push_back('\0');

    STARTUPINFOA si = { sizeof(si) };
    PROCESS_INFORMATION pi;

    if (CreateProcessA(
        loginExe.c_str(),
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
    /*if (!pipe) return "";

    char buffer[2000];
    std::string result;
    while (fgets(buffer, sizeof(buffer), pipe))
        result += buffer;

    _pclose(pipe);*/

    std::ifstream file("login_info.txt");
    if (!file.is_open()) return "";

    std::string result;
    std::getline(file, result);

    file.close();

    if (!result.empty() && (result.back() == '\n' || result.back() == '\r'))
        result.pop_back();

    std::vector<std::string> parts;
    size_t start = 0;
    size_t found;

    while ((found = result.find('|', start)) != std::string::npos) {
        parts.push_back(result.substr(start, found - start));
        start = found + 1;
    }
    parts.push_back(result.substr(start));

    if (parts.size() == 4) {
        status = parts[0];
        id = parts[1];
        token = parts[2];
        name = parts[3];
    }

    /*if (id == "" || token == "") {
        status = "failed";
    }*/

    return status;
}

std::string runRegisterFormAndReadResult() {
    char exePath[MAX_PATH];
    GetModuleFileNameA(NULL, exePath, MAX_PATH);

    std::string currentDir = exePath;
    size_t pos = currentDir.find_last_of("\\/");
    if (pos != std::string::npos)
        currentDir = currentDir.substr(0, pos);

    std::string projectRoot = currentDir.substr(0, currentDir.find_last_of("\\/"));
    projectRoot = projectRoot.substr(0, projectRoot.find_last_of("\\/"));

    std::string registerExe = projectRoot + "\\Register\\bin\\Debug\\Register.exe";

    std::string cmdLine = "\"" + registerExe + "\"";
    std::vector<char> cmdLineBuffer(cmdLine.begin(), cmdLine.end());
    cmdLineBuffer.push_back('\0');

    STARTUPINFOA si = { sizeof(si) };
    PROCESS_INFORMATION pi;

    if (CreateProcessA(
        registerExe.c_str(),
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

    /*char buffer[128];
    std::string result;
    while (fgets(buffer, sizeof(buffer), pipe))
        result += buffer;

    _pclose(pipe);*/

    std::ifstream file("register_info.txt");
    if (!file.is_open()) return "";

    std::string result;
    std::getline(file, result);

    file.close();

    if (!result.empty() && (result.back() == '\n' || result.back() == '\r'))
        result.pop_back();

    return result;
}

void LoginMenu::enter() {
    if (!inLoginForm && !inRegisterForm) {
        switch (activeMenuOption) {
        case 0: {
            std::string id, token, name;
            inLoginForm = true;

            std::string status = runLoginFormAndReadResult(id, token, name);
            if (status == "success") {
                loginFailed = false;
                CCFG::setLocalId(id);
                CCFG::setIdToken(token);
                CCFG::setUserName(name);
                CCFG::getMM()->resetActiveOptionID(CCFG::getMM()->eMainMenu);
                CCFG::getMM()->setViewID(CCFG::getMM()->eMainMenu);
            }
            else {
                loginFailed = true;
            }
            inLoginForm = false;
            break;
        }
        case 1:
            inRegisterForm = true;

            std::string status = runRegisterFormAndReadResult();
            if (status == "success") {
                registerFailed = false;
                CCFG::getMM()->setViewID(CCFG::getMM()->eLoginMenu);
            }
            else {
                registerFailed = true;
            }
            inRegisterForm = false;
            break;
        }
    }
    else {
        CCFG::getMM()->setViewID(CCFG::getMM()->eLoginMenu);
    }
}

void LoginMenu::escape() {
    if (inLoginForm) {
        inLoginForm = false;
        loginFailed = false;
    }
    else if (inRegisterForm) {
        inRegisterForm = false;
        registerFailed = false;
    }
}

void LoginMenu::updateActiveButton(int iDir) {
    switch (iDir) {
    case 0: case 2:
        Menu::updateActiveButton(iDir);
        break;
    }
}