#include "SaveHistory.h"
#include <curl/curl.h>
#include <string>

bool SaveHistory(
    const std::string& uid,
    const std::string& idToken,
    const std::string& mode,
    const std::string& opponent,
    const std::string& result,
    int score,
    int duration
)
{
    CURL* curl = curl_easy_init();
    if (!curl) return false;

    // URL API của server
    std::string url = "https://localhost:7244/api/Firebase/saveHistory";

    // JSON body
    std::string jsonData = "{";
    jsonData += "\"localId\":\"" + uid + "\",";
    jsonData += "\"idToken\":\"" + idToken + "\",";
    jsonData += "\"mode\":\"" + mode + "\",";
    jsonData += "\"opponent\":\"" + opponent + "\",";
    jsonData += "\"result\":\"" + result + "\",";
    jsonData += "\"score\":" + std::to_string(score) + ",";
    jsonData += "\"duration\":" + std::to_string(duration);
    jsonData += "}";

    struct curl_slist* headers = nullptr;
    headers = curl_slist_append(headers, "Content-Type: application/json");

    curl_easy_setopt(curl, CURLOPT_URL, url.c_str());
    curl_easy_setopt(curl, CURLOPT_POSTFIELDS, jsonData.c_str());
    curl_easy_setopt(curl, CURLOPT_HTTPHEADER, headers);

    // Bỏ kiểm tra SSL (chỉ dùng thử localhost)
    curl_easy_setopt(curl, CURLOPT_SSL_VERIFYPEER, 0L);
    curl_easy_setopt(curl, CURLOPT_SSL_VERIFYHOST, 0L);

    CURLcode res = curl_easy_perform(curl);

    curl_slist_free_all(headers);
    curl_easy_cleanup(curl);

    return res == CURLE_OK;
}
