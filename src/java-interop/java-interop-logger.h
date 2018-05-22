#ifndef __JAVA_INTEROP_LOGGER_H__
#define __JAVA_INTEROP_LOGGER_H__

#ifndef ANDROID
typedef enum android_LogPriority {
    ANDROID_LOG_UNKNOWN = 0,
    ANDROID_LOG_DEFAULT,    /* only for SetMinPriority() */
    ANDROID_LOG_VERBOSE,
    ANDROID_LOG_DEBUG,
    ANDROID_LOG_INFO,
    ANDROID_LOG_WARN,
    ANDROID_LOG_ERROR,
    ANDROID_LOG_FATAL,
    ANDROID_LOG_SILENT,     /* only for SetMinPriority(); must be last */
} android_LogPriority;
#endif

// Keep in sync with Mono.Android/src/Runtime/Logger.cs!LogCategories enum
typedef enum _LogCategories {
	LOG_NONE      = 0,
	LOG_DEFAULT   = 1 << 0,
	LOG_ASSEMBLY  = 1 << 1,
	LOG_DEBUGGER  = 1 << 2,
	LOG_GC        = 1 << 3,
	LOG_GREF      = 1 << 4,
	LOG_LREF      = 1 << 5,
	LOG_TIMING    = 1 << 6,
	LOG_BUNDLE    = 1 << 7,
	LOG_NET       = 1 << 8,
	LOG_NETLINK   = 1 << 9,
} LogCategories;

extern unsigned int log_categories;

#if DEBUG
extern int gc_spew_enabled;
#endif

void init_categories (const char *override_dir);

void log_error (LogCategories category, const char *format, ...);

void log_fatal (LogCategories category, const char *format, ...);

void log_info (LogCategories category, const char *format, ...);

void log_warn (LogCategories category, const char *format, ...);

void log_debug (LogCategories category, const char *format, ...);

#endif /* __JAVA_INTEROP_LOGGER_H__ */
