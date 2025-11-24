#include "header.h"
#include "Core.h"
#include "APIManager.h"


int main(int argc, char *argv[])
{
	CCore oCore;

	oCore.mainLoop();

	APIManager api;
	api.CreateRoom("PhongMarioVip");

	return 0;
}
