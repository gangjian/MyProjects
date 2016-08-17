#if defined (START_WITH_IF)
/* swc_in_trcta_MemMap (Default Section Mapping) */

/* CODE section */
#elif defined (swc_in_trcta_START_SEC_CODE)
   #undef      swc_in_trcta_START_SEC_CODE
   #define DEFAULT_START_SEC_CODE
#elif defined (swc_in_trcta_STOP_SEC_CODE)
   #undef      swc_in_trcta_STOP_SEC_CODE
   #define DEFAULT_STOP_SEC_CODE
   
/* VARIABLE section ARRAY,STRUCTURE*/
#elif defined (swc_in_trcta_START_SEC_VAR_INIT_UNSPECIFIED)
   #undef      swc_in_trcta_START_SEC_VAR_INIT_UNSPECIFIED
   #define DEFAULT_START_SEC_VAR_INIT_UNSPECIFIED
#elif defined (swc_in_trcta_STOP_SEC_VAR_INIT_UNSPECIFIED)
   #undef      swc_in_trcta_STOP_SEC_VAR_INIT_UNSPECIFIED
   #define DEFAULT_STOP_SEC_VAR_INIT_UNSPECIFIED
   
/* VARIABLE section uint8*/
#elif defined (swc_in_trcta_START_SEC_VAR_INIT_8)
   #undef      swc_in_trcta_START_SEC_VAR_INIT_8
   #define DEFAULT_START_SEC_VAR_INIT_8
#elif defined (swc_in_trcta_STOP_SEC_VAR_INIT_8)
   #undef      swc_in_trcta_STOP_SEC_VAR_INIT_8
   #define DEFAULT_STOP_SEC_VAR_INIT_8
   
/* VARIABLE section uint16*/
#elif defined (swc_in_trcta_START_SEC_VAR_INIT_16)
   #undef      swc_in_trcta_START_SEC_VAR_INIT_16
   #define DEFAULT_START_SEC_VAR_INIT_16
#elif defined (swc_in_trcta_STOP_SEC_VAR_INIT_16)
   #undef      swc_in_trcta_STOP_SEC_VAR_INIT_16
   #define DEFAULT_STOP_SEC_VAR_INIT_16
   
/* VARIABLE section uint32*/
#elif defined (swc_in_trcta_START_SEC_VAR_INIT_32)
   #undef      swc_in_trcta_START_SEC_VAR_INIT_32
   #define DEFAULT_START_SEC_VAR_INIT_32
#elif defined (swc_in_trcta_STOP_SEC_VAR_INIT_32)
   #undef      swc_in_trcta_STOP_SEC_VAR_INIT_32
   #define DEFAULT_STOP_SEC_VAR_INIT_32
   
/* CONST section ARRAY,STRUCTURE*/
#elif defined (swc_in_trcta_START_SEC_CONST_UNSPECIFIED)
   #undef      swc_in_trcta_START_SEC_CONST_UNSPECIFIED
   #define DEFAULT_START_SEC_CONST_UNSPECIFIED
#elif defined (swc_in_trcta_STOP_SEC_CONST_UNSPECIFIED)
   #undef      swc_in_trcta_STOP_SEC_CONST_UNSPECIFIED
   #define DEFAULT_STOP_SEC_CONST_UNSPECIFIED
   
/* CONST section uint8*/
#elif defined (swc_in_trcta_START_SEC_CONST_8)
   #undef      swc_in_trcta_START_SEC_CONST_8
   #define DEFAULT_START_SEC_CONST_8
#elif defined (swc_in_trcta_STOP_SEC_CONST_8)
   #undef      swc_in_trcta_STOP_SEC_CONST_8
   #define DEFAULT_STOP_SEC_CONST_8
   
/* CONST section uint16*/
#elif defined (swc_in_trcta_START_SEC_CONST_16)
   #undef      swc_in_trcta_START_SEC_CONST_16
   #define DEFAULT_START_SEC_CONST_16
#elif defined (swc_in_trcta_STOP_SEC_CONST_16)
   #undef      swc_in_trcta_STOP_SEC_CONST_16
   #define DEFAULT_STOP_SEC_CONST_16
   
/* CONST section uint32*/
#elif defined (swc_in_trcta_START_SEC_CONST_32)
   #undef      swc_in_trcta_START_SEC_CONST_32
   #define DEFAULT_START_SEC_CONST_32
#elif defined (swc_in_trcta_STOP_SEC_CONST_32)
   #undef      swc_in_trcta_STOP_SEC_CONST_32
   #define DEFAULT_STOP_SEC_CONST_32

#else
   #error "swc_in_trcta_MemMap.h: No valid section define found"
#endif  /* START_WITH_IF */

#include "default_MemMap.h"

