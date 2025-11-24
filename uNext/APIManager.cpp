#include "APIManager.h"

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
        // Tạo đường dẫn: http://localhost:5000/api/rooms?name=TenPhong
        // Lưu ý: Nếu tên phòng có dấu cách, cần xử lý encode (nhưng tạm thời cứ viết liền không dấu)
        std::string url = BASE_URL + "?name=" + roomName;

        curl_easy_setopt(curl, CURLOPT_URL, url.c_str());
        curl_easy_setopt(curl, CURLOPT_POST, 1L); // Báo đây là lệnh POST
        curl_easy_setopt(curl, CURLOPT_POSTFIELDSIZE, 0L); // Không có body, chỉ có query param

        // Thiết lập hàm hứng phản hồi
        curl_easy_setopt(curl, CURLOPT_WRITEFUNCTION, WriteCallback);
        curl_easy_setopt(curl, CURLOPT_WRITEDATA, &readBuffer);

        // Gửi đi!
        res = curl_easy_perform(curl);

        if (res != CURLE_OK) {
            std::cerr << "curl_easy_perform() failed: " << curl_easy_strerror(res) << std::endl;
        }
        else {
            std::cout << "Tao phong thanh cong! Server tra loi: " << readBuffer << std::endl;
            // Ở đây bạn sẽ cần code để tách lấy cái "id" trong chuỗi readBuffer (JSON)
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