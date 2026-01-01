#include "APIManager.h"
bool APIManager::isRoomOwner = false;
std::string APIManager::currentRoomID = "";

APIManager::APIManager() {
    curl_global_init(CURL_GLOBAL_ALL); // Khởi động curl
}

APIManager::~APIManager() {
    curl_global_cleanup(); // Dọn dẹp khi tắt game
}

// Hàm nhận phản hồi từ Server (Bắt buộc phải có khi dùng libcurl)
size_t APIManager::WriteCallback(void* contents, size_t size, size_t nmemb, void* userp) {
    ((std::string*)userp)->append((char*)contents, size * nmemb);
    return size * nmemb;
}

void APIManager::CreateRoom(std::string roomName) {
    CURL* curl;
    CURLcode res;
    std::string readBuffer;

    curl = curl_easy_init();
    if (curl) {
        std::string url = BASE_URL + "/" + roomName;
        curl_easy_setopt(curl, CURLOPT_URL, url.c_str());
        curl_easy_setopt(curl, CURLOPT_POST, 1L);
        curl_easy_setopt(curl, CURLOPT_POSTFIELDSIZE, 0L);

        curl_easy_setopt(curl, CURLOPT_WRITEFUNCTION, WriteCallback);
        curl_easy_setopt(curl, CURLOPT_WRITEDATA, &readBuffer);

        res = curl_easy_perform(curl);

        if (res != CURLE_OK) {
            std::cerr << "Loi ket noi: " << curl_easy_strerror(res) << std::endl;
        }
        else {
            // readBuffer lúc này có dạng: {"id":"-Nk123abc...","name":"PhongVip",...}

            // --- ĐOẠN CODE MỚI: BÓC TÁCH ID ---
            std::string key = "\"id\":\""; // Chuỗi cần tìm: "id":"
            size_t startPos = readBuffer.find(key);

            if (startPos != std::string::npos) {
                // Tìm thấy chữ "id":", dịch con trỏ sang phải để lấy nội dung
                startPos += key.length();

                // Tìm dấu ngoặc kép đóng "
                size_t endPos = readBuffer.find("\"", startPos);

                if (endPos != std::string::npos) {
                    // Cắt chuỗi từ startPos đến endPos
                    std::string roomId = readBuffer.substr(startPos, endPos - startPos);

                    std::cout << "\n************************************************" << std::endl;
                    std::cout << " TAO PHONG THANH CONG!" << std::endl;
                    std::cout << " TEN PHONG: " << roomName << std::endl;
                    std::cout << " ID PHONG : " << roomId << "  <--- (Copy cai nay dua cho ban be)" << std::endl;
                    std::cout << "************************************************\n" << std::endl;
                }
            }
            else {
                // Nếu không bóc tách được thì in nguyên cục ra để xem lỗi
                std::cout << "Server tra ve: " << readBuffer << std::endl;
            }
            // -----------------------------------
        }
        curl_easy_cleanup(curl);
    }
}
void APIManager::DeleteRoom(std::string roomId) {
    CURL* curl;
    CURLcode res;
    std::string readBuffer;

    curl = curl_easy_init();
    if (curl) {
        // Tạo đường dẫn: http://localhost:5000/api/rooms/ID_CUA_PHONG
        std::string url = BASE_URL + "/" + roomId;

        curl_easy_setopt(curl, CURLOPT_URL, url.c_str());
        curl_easy_setopt(curl, CURLOPT_CUSTOMREQUEST, "DELETE"); // Báo đây là lệnh DELETE

        curl_easy_setopt(curl, CURLOPT_WRITEFUNCTION, WriteCallback);
        curl_easy_setopt(curl, CURLOPT_WRITEDATA, &readBuffer);

        res = curl_easy_perform(curl);

        if (res != CURLE_OK)
            std::cerr << "Xoa that bai: " << curl_easy_strerror(res) << std::endl;
        else
            std::cout << "Xoa phong xong! Server tra loi: " << readBuffer << std::endl;

        curl_easy_cleanup(curl);
    }
}
bool APIManager::JoinRoom(std::string roomId) {
    CURL* curl;
    CURLcode res;
    std::string readBuffer;
    long http_code = 0;

    curl = curl_easy_init();
    if (curl) {
        // Đường dẫn: http://localhost:5000/api/rooms/join/ID_PHONG
        std::string url = BASE_URL + "/join/" + roomId;

        curl_easy_setopt(curl, CURLOPT_URL, url.c_str());
        curl_easy_setopt(curl, CURLOPT_POST, 1L); // Lệnh POST
        curl_easy_setopt(curl, CURLOPT_POSTFIELDSIZE, 0L); // Không cần gửi body

        curl_easy_setopt(curl, CURLOPT_WRITEFUNCTION, WriteCallback);
        curl_easy_setopt(curl, CURLOPT_WRITEDATA, &readBuffer);

        res = curl_easy_perform(curl);

        // Lấy mã phản hồi HTTP (200 là OK, 404 là không tìm thấy)
        curl_easy_getinfo(curl, CURLINFO_RESPONSE_CODE, &http_code);

        curl_easy_cleanup(curl);

        if (res == CURLE_OK && http_code == 200) {
            std::cout << "[SUCCESS] Da vao phong! " << readBuffer << std::endl;
            return true; // Vào thành công
        }
        else {
            std::cout << "[ERROR] Khong vao duoc phong! Ma loi: " << http_code << std::endl;
            return false; // Thất bại
        }
    }
    return false;
}