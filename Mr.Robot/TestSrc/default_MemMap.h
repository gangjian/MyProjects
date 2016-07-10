#if defined (START_WITH_IF)
/* CODE section */
#elif defined (DEFAULT_STOP_SEC_CODE)
   #ifdef DEFAULT_START_SEC_CODE
      #undef DEFAULT_START_SEC_CODE
      #undef SECTION_STARTED
      #undef DEFAULT_STOP_SEC_CODE
   #else
      #error "DEFAULT_STOP_SEC_CODE has not been previously started."
   #endif
#elif defined (DEFAULT_START_SEC_CODE)
   #ifndef SECTION_STARTED
      #define SECTION_STARTED
   #else
      #error "DEFAULT_START_SEC_CODE has already been started."
   #endif
/* VARIABLE section ARRAY,STRUCTURE*/
#elif defined (DEFAULT_STOP_SEC_VAR_INIT_UNSPECIFIED)
   #ifdef DEFAULT_START_SEC_VAR_INIT_UNSPECIFIED
      #undef DEFAULT_START_SEC_VAR_INIT_UNSPECIFIED
      #undef SECTION_STARTED
      #undef DEFAULT_STOP_SEC_VAR_INIT_UNSPECIFIED
   #else
      #error "DEFAULT_STOP_SEC_VAR_INIT_UNSPECIFIED has not been previously started."
   #endif
#elif defined (DEFAULT_START_SEC_VAR_INIT_UNSPECIFIED)
   #ifndef SECTION_STARTED
      #define SECTION_STARTED
   #else
      #error "DEFAULT_START_SEC_VAR_INIT_8 has already been started."
   #endif
/* VARIABLE section uint8*/
#elif defined (DEFAULT_STOP_SEC_VAR_INIT_8)
   #ifdef DEFAULT_START_SEC_VAR_INIT_8
      #undef DEFAULT_START_SEC_VAR_INIT_8
      #undef SECTION_STARTED
      #undef DEFAULT_STOP_SEC_VAR_INIT_8
   #else
      #error "DEFAULT_STOP_SEC_VAR_INIT_8 has not been previously started."
   #endif
#elif defined (DEFAULT_START_SEC_VAR_INIT_8)
   #ifndef SECTION_STARTED
      #define SECTION_STARTED
   #else
      #error "DEFAULT_START_SEC_VAR_INIT_8 has already been started."
   #endif
/* VARIABLE section uint16*/
#elif defined (DEFAULT_STOP_SEC_VAR_INIT_16)
   #ifdef DEFAULT_START_SEC_VAR_INIT_16
      #undef DEFAULT_START_SEC_VAR_INIT_16
      #undef SECTION_STARTED
      #undef DEFAULT_STOP_SEC_VAR_INIT_16
   #else
      #error "DEFAULT_STOP_SEC_VAR_INIT_16 has not been previously started."
   #endif
#elif defined (DEFAULT_START_SEC_VAR_INIT_16)
   #ifndef SECTION_STARTED
      #define SECTION_STARTED
   #else
      #error "DEFAULT_START_SEC_VAR_INIT_16 has already been started."
   #endif
/* VARIABLE section uint32*/
#elif defined (DEFAULT_STOP_SEC_VAR_INIT_32)
   #ifdef DEFAULT_START_SEC_VAR_INIT_32
      #undef DEFAULT_START_SEC_VAR_INIT_32
      #undef SECTION_STARTED
      #undef DEFAULT_STOP_SEC_VAR_INIT_32
   #else
      #error "DEFAULT_STOP_SEC_VAR_INIT_32 has not been previously started."
   #endif
#elif defined (DEFAULT_START_SEC_VAR_INIT_32)
   #ifndef SECTION_STARTED
      #define SECTION_STARTED
   #else
      #error "DEFAULT_START_SEC_VAR_INIT_32 has already been started."
   #endif
/* CONST section ARRAY,STRUCTURE*/
#elif defined (DEFAULT_STOP_SEC_CONST_UNSPECIFIED)
   #ifdef DEFAULT_START_SEC_CONST_UNSPECIFIED
      #undef DEFAULT_START_SEC_CONST_UNSPECIFIED
      #undef SECTION_STARTED
      #undef DEFAULT_STOP_SEC_CONST_UNSPECIFIED
   #else
      #error "DEFAULT_STOP_SEC_CONST_UNSPECIFIED has not been previously started."
   #endif
#elif defined (DEFAULT_START_SEC_CONST_UNSPECIFIED)
   #ifndef SECTION_STARTED
      #define SECTION_STARTED
   #else
      #error "DEFAULT_START_SEC_CONST_UNSPECIFIED has already been started."
   #endif
/* CONST section uint8*/
#elif defined (DEFAULT_STOP_SEC_CONST_8)
   #ifdef DEFAULT_START_SEC_CONST_8
      #undef DEFAULT_START_SEC_CONST_8
      #undef SECTION_STARTED
      #undef DEFAULT_STOP_SEC_CONST_8
   #else
      #error "DEFAULT_STOP_SEC_CONST_8 has not been previously started."
   #endif
#elif defined (DEFAULT_START_SEC_CONST_8)
   #ifndef SECTION_STARTED
      #define SECTION_STARTED
   #else
      #error "DEFAULT_START_SEC_CONST_8 has already been started."
   #endif
/* CONST section uint16*/
#elif defined (DEFAULT_STOP_SEC_CONST_16)
   #ifdef DEFAULT_START_SEC_CONST_16
      #undef DEFAULT_START_SEC_CONST_16
      #undef SECTION_STARTED
      #undef DEFAULT_STOP_SEC_CONST_16
   #else
      #error "DEFAULT_STOP_SEC_CONST_16 has not been previously started."
   #endif
#elif defined (DEFAULT_START_SEC_CONST_16)
   #ifndef SECTION_STARTED
      #define SECTION_STARTED
   #else
      #error "DEFAULT_START_SEC_CONST_16 has already been started."
   #endif
/* CONST section uint32*/
#elif defined (DEFAULT_STOP_SEC_CONST_32)
   #ifdef DEFAULT_START_SEC_CONST_32
      #undef DEFAULT_START_SEC_CONST_32
      #undef SECTION_STARTED
      #undef DEFAULT_STOP_SEC_CONST_32
   #else
      #error "DEFAULT_STOP_SEC_CONST_32 has not been previously started."
   #endif
#elif defined (DEFAULT_START_SEC_CONST_32)
   #ifndef SECTION_STARTED
      #define SECTION_STARTED
   #else
      #error "DEFAULT_START_SEC_CONST_32 has already been started."
   #endif

#else
   #error "No valid default section define found."
#endif  /* START_WITH_IF */

