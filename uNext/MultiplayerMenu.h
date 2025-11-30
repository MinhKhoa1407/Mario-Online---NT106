#pragma once
#ifndef MULTIPLAYERMENU_H
#define MULTIPLAYERMENU_H

#include "Menu.h"

class MultiplayerMenu : public Menu
{
public:
    MultiplayerMenu(void);
    ~MultiplayerMenu(void);

    void Update();
    void Draw(SDL_Renderer* rR);

    void enter();
    void escape();
    void updateActiveButton(int iDir);

    // Hàm gọi Form C# kèm tham số
    void OpenLauncher(std::string mode, std::string username);
};

#endif