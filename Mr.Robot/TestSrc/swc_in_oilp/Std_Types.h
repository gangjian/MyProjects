/******************************************************************************
 *
 *                    Mentor Graphics Corporation
 *                        All rights reserved
 *
 ******************************************************************************
 *
 * Module:       Common\Platform
 *
 * File Name:    Std_Types.h
 *
 * Description:  General type definitions and macros
 *
 *
 ******************************************************************************/

#ifndef STD_TYPES_H
#define STD_TYPES_H

/*=============================== INCLUSIONS =================================*/

#include "Compiler.h"
#include "Platform_Types.h"

#ifdef __cplusplus
extern "C"
{
#endif

/* Published information */
#define STD_TYPES_VENDOR_ID                     ((uint16) 31)
/*#define STD_TYPES_AR_RELEASE_MAJOR_VERSION (4u)*/
#define STD_TYPES_AR_RELEASE_MAJOR_VERSION (4u)
/*#define STD_TYPES_AR_RELEASE_MINOR_VERSION (0u)*/
#define STD_TYPES_AR_RELEASE_MINOR_VERSION (0u)
#define STD_TYPES_AR_RELEASE_REVISION_VERSION (2u)
#define STD_TYPES_SW_MAJOR_VERSION (1u)
#define STD_TYPES_SW_MINOR_VERSION (0u)
#define STD_TYPES_SW_PATCH_VERSION (0u)

/*========================= COMPILER SETTING CHECKS ==========================*/

/*============================ TYPE DEFINITIONS ==============================*/

/**
 * \defgroup Std_Types Type Definitions
 *  Describes the standard Type Definitions used in the project
 */

/* \brief standard return type */
typedef uint8  Std_ReturnType;

/* /brief Structure for the Version of the module.
*         This is requested by calling <Module name>_GetVersionInfo() */

typedef struct
{
  uint16  vendorID;
  uint16  moduleID;
  uint8   sw_major_version;
  uint8   sw_minor_version;
  uint8   sw_patch_version;
}Std_VersionInfoType;

#if (defined E_EARLIER_ACTIVE) /* guard to prevent double definition */
#if (E_EARLIER_ACTIVE != 3U)
#error E_EARLIER_ACTIVE does not have value 3
#endif /* if (E_NOT_OK != 1U) */
#else
#define E_EARLIER_ACTIVE 3U
#endif /* if (defined E_NOT_OK) */


#if (defined E_PAST) /* guard to prevent double definition */
#if (E_PAST != 4U)
#error E_PAST does not have value 4
#endif /* if (E_NOT_OK != 4U) */
#else
#define E_PAST 4U
#endif /* if (defined E_NOT_OK) */

#if (defined E_NOT_ACTIVE) /* guard to prevent double definition */
#if (E_NOT_ACTIVE != 5U)
#error E_NOT_ACTIVE does not have value 5
#endif /* if (E_NOT_ACTIVE != 5U) */
#else
#define E_NOT_ACTIVE 5U
#endif /* if (defined E_NOT_ACTIVE) */

/*============================ MACRO DEFINITIONS =============================*/

#ifndef STATUSTYPEDEFINED
  #define STATUSTYPEDEFINED
  #define E_OK     0x00

  typedef unsigned char StatusType; /* OSEK compliance */
#endif

#define E_NOT_OK   0x01

#define STD_HIGH   0x01       /**< Standard HIGH */
#define STD_LOW    0x00       /**< Standard LOW */

#define STD_ACTIVE 0x01       /**< Logical state active */
#define STD_IDLE   0x00       /**< Logical state idle */

#define STD_ON     0x01       /**< Standard ON */
#define STD_OFF    0x00       /**< Standard OFF */

#ifdef __cplusplus
}
#endif

#endif /*STD_TYPES_H*/
