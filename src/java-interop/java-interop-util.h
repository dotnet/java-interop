#ifdef WINDOWS
/* Those two conversion functions are only properly implemented on Windows
 * because that's the only place where they should be useful.
 */
char* utf16_to_utf8 (const wchar_t *widestr);
wchar_t* utf8_to_utf16 (const char *mbstr);
#endif // def WINDOWS
