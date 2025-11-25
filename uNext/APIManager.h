#pragma once
#include <string>
#include <iostream>
#include <curl.h> 

class APIManager
{
public:
    // Địa chỉ Server C# (Lát nữa chạy API lên mới biết cổng nào, ví dụ 5000 hay 7070)
    // Lưu ý: localhost nghĩa là máy của bạn.
    const std::string BASE_URL = "http://localhost:5000/api/rooms";
    static bool isRoomOwner;       // Biến dùng chung cho toàn game
    static std::string currentRoomID;
    APIManager();
    ~APIManager();

    // Hàm tạo phòng
    void CreateRoom(std::string roomName);

    // Hàm xóa phòng
    void DeleteRoom(std::string roomId);
    bool JoinRoom(std::string roomId);

private:
    // Hàm phụ trợ để nhận dữ liệu trả về từ Server (Callback)
    static size_t WriteCallback(void* contents, size_t size, size_t nmemb, void* userp);
};