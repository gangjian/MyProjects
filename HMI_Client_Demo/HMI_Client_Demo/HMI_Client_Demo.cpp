// HMI_Client_Demo.cpp : 定义控制台应用程序的入口点。
//

#include "stdafx.h"
#include <WINSOCK2.H>
#include <STDIO.H>
#include <string>

using  std::string;
using  std::wstring;

#pragma  comment(lib, "ws2_32.lib")

#define  _WINSOCK_DEPRECATED_NO_WARNINGS

extern char * WCHAR_TP2CHARP(wchar_t *WStr);
extern void SendData2HimSim(wchar_t *WStr);

int _tmain(int argc, _TCHAR* argv[])
{
	if (argc > 1)
	{
		_TCHAR* p = argv[1];
		//wchar_t *WStr = L"0123,456789残り時間!";
		wchar_t *WStr = p;
		SendData2HimSim(WStr);
	}
	return 0;
}

void SendData2HimSim(wchar_t *WStr)
{
	WORD sockVersion = MAKEWORD(2, 2);
	WSADATA data;
	if (WSAStartup(sockVersion, &data) != 0)
	{
		return;
	}

	SOCKET sclient = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
	if (sclient == INVALID_SOCKET)
	{
		printf("invalid socket !");
		return;
	}

	sockaddr_in serAddr;
	serAddr.sin_family = AF_INET;
	serAddr.sin_port = htons(2017);
	serAddr.sin_addr.S_un.S_addr = inet_addr("127.0.0.1");
	if (connect(sclient, (sockaddr *)&serAddr, sizeof(serAddr)) == SOCKET_ERROR)
	{
		printf("connect error !");
		closesocket(sclient);
		return;
	}
	char * sendData = WCHAR_TP2CHARP(WStr);
	send(sclient, sendData, strlen(sendData), 0);

	//char recData[255];
	//int ret = recv(sclient, recData, 255, 0);
	//if (ret > 0)
	//{
	//	recData[ret] = 0x00;
	//	printf(recData);
	//}
	closesocket(sclient);
	WSACleanup();
}

char * WCHAR_TP2CHARP(wchar_t *WStr)
{
	setlocale(LC_ALL, "");
	size_t len = 200;
	size_t converted = 0;
	char *cp;
	cp = (char*)malloc(len*sizeof(char));
	wcstombs_s(&converted, cp, len, WStr, len);
	return cp;
}

