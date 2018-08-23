#include <stdlib.h>
#include <stdarg.h>
#include <stdio.h>
#include <strings.h>
#include <string.h>
#include <unistd.h>
#include <errno.h>

#ifdef ANDROID
#include <android/log.h>
#endif

#include "java-interop-logger.h"

#define DO_LOG(_level_,_category_,_format_,_args_)						                        \
	va_start ((_args_), (_format_));									                        \
	__android_log_vprint ((_level_), CATEGORY_NAME((_category_)), (_format_), (_args_)); \
	va_end ((_args_));

// Must match the same ordering as LogCategories
static const char* log_names[] = {
	"*none*",
	"monodroid",
	"monodroid-assembly",
	"monodroid-debug",
	"monodroid-gc",
	"monodroid-gref",
	"monodroid-lref",
	"monodroid-timing",
	"monodroid-bundle",
	"monodroid-network",
	"monodroid-netlink",
	"*error*",
};

#if defined(__i386__) && defined(__GNUC__)
#define ffs(__value__) __builtin_ffs ((__value__))
#elif defined(__x86_64__) && defined(__GNUC__)
#define ffs(__value__) __builtin_ffsll ((__value__))
#endif

// ffs(value) returns index of lowest bit set in `value`
#define CATEGORY_NAME(value) (value == 0 ? log_names [0] : log_names [ffs (value)])

#ifndef ANDROID
static void
__android_log_vprint (int prio, const char* tag, const char* fmt, va_list ap)
{
  printf ("%d [%s] ", prio, tag);
  vprintf (fmt, ap);
  putchar ('\n');
  fflush (stdout);
}
#endif

unsigned int log_categories;
int gc_spew_enabled;

void
log_error (LogCategories category, const char *format, ...)
{
	va_list args;

	DO_LOG (ANDROID_LOG_ERROR, category, format, args);
}

void
log_fatal (LogCategories category, const char *format, ...)
{
	va_list args;

	DO_LOG (ANDROID_LOG_FATAL, category, format, args);
}

void
log_info (LogCategories category, const char *format, ...)
{
	va_list args;

	if ((log_categories & category) == 0)
		return;

	DO_LOG (ANDROID_LOG_INFO, category, format, args);
}

void
log_warn (LogCategories category, const char *format, ...)
{
	va_list args;

	DO_LOG (ANDROID_LOG_WARN, category, format, args);
}

void
log_debug (LogCategories category, const char *format, ...)
{
	va_list args;

	if ((log_categories & category) == 0)
		return;

	DO_LOG (ANDROID_LOG_DEBUG, category, format, args);
}
