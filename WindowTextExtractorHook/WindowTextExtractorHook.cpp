// WindowTextExtractorHook.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include "tchar.h"
#include "WindowTextExtractorHook.h"

// Instruct the compiler to put the g_hXXXhook data variable in 
// its own data section called Shared. We then instruct the 
// linker that we want to share the data in this section 
// with all instances of this application.
#pragma data_seg("Shared")
HHOOK g_hHook = NULL;
HWND  g_hwndCaller = NULL;
HWND  g_hwndTarget = NULL;
UINT  g_msg = 0;
#pragma data_seg()

// Instruct the linker to make the Shared section readable, writable, and shared.
#pragma comment(linker, "/section:Shared,rws")

HINSTANCE g_hinstanceDll = NULL;

LRESULT CALLBACK CallWndProcHookCallback(int nCode, WPARAM wParam, LPARAM lParam);

BOOL WINAPI SetHook(HWND hwndCaller, HWND hwndTarget, UINT uMsg)
{
	if (g_hHook)
	{
		::UnsetHook(g_hwndCaller, g_hwndTarget);
	}
	g_hHook = SetWindowsHookEx(WH_CALLWNDPROC, (HOOKPROC)CallWndProcHookCallback, g_hinstanceDll, GetWindowThreadProcessId(hwndTarget, NULL));
	if (g_hHook == NULL)
	{
		return FALSE;
	}
	g_hwndCaller = hwndCaller;
	g_hwndTarget = hwndTarget;
	g_msg = uMsg;
	return TRUE;
}

BOOL WINAPI UnsetHook(HWND hwndCaller, HWND hwndTarget)
{
	if (g_hHook)
	{
		return UnhookWindowsHookEx(g_hHook);
	}
	return TRUE;
}

BOOL WINAPI QueryPasswordEdit()
{
	if (g_hHook == NULL || g_hwndCaller == NULL || g_hwndTarget == NULL || g_msg == 0)
	{
		return FALSE;
	}
	::SendMessage(g_hwndTarget, g_msg, 0, 0);
	return TRUE;
}

LRESULT CALLBACK CallWndProcHookCallback(int nCode, WPARAM wParam, LPARAM lParam)
{
	if (nCode >= 0)
	{
		CWPSTRUCT* pCwp = (CWPSTRUCT*)lParam;
		if (pCwp->message == g_msg && pCwp->hwnd == g_hwndTarget && pCwp->wParam == 0)
		{
			TCHAR szBuffer[4096] = { _T('\0') };
			SendMessage(g_hwndTarget, WM_GETTEXT, sizeof(szBuffer) / sizeof(TCHAR), (LPARAM)szBuffer);
			COPYDATASTRUCT cds = { 0 };
			cds.dwData = HandleToUlong(g_hwndTarget);
			cds.cbData = (lstrlen(szBuffer) + 1) * sizeof(TCHAR);
			cds.lpData = szBuffer;
			SendMessage(g_hwndCaller, WM_COPYDATA, (WPARAM)g_hwndTarget, (LPARAM)&cds);
			//TCHAR buf[256];
			//wsprintf(buf, L"CallWndProcHookCallback cds.dwData = %d, cds.cbData = %d, szBuffer = %s", cds.dwData, cds.cbData, szBuffer);
			//OutputDebugString(buf);
		}
	}
	return CallNextHookEx(g_hHook, nCode, wParam, lParam);
}