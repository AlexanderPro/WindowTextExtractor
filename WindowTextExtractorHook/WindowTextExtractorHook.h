
#ifndef	_WINDOW_TEXT_EXTRACT_HOOK_H_
#define	_WINDOW_TEXT_EXTRACT_HOOK_H_

#include <windows.h>

#define DLLEXPORT extern "C" __declspec(dllexport)

DLLEXPORT BOOL __stdcall WINAPI SetHook(HWND hwndCaller, HWND hwndTarget, UINT uMsg);
DLLEXPORT BOOL __stdcall WINAPI UnsetHook(HWND hWndCaller, HWND hWndTarget);
DLLEXPORT BOOL __stdcall WINAPI QueryPasswordEdit();

#endif