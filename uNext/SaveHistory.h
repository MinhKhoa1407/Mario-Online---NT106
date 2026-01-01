#pragma once
#include <string>

bool SaveHistory(
    const std::string& uid,
    const std::string& idToken,
    const std::string& mode,
    const std::string& opponent,
    const std::string& result,
    int score,
    int duration
);
