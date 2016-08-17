/** \file
 *
 * \brief Generated file for Rte
 *
 * 
 *
 * \b Application:        Rte \n
 * \b Target:             see Rte.h for details \n
 * \b Compiler:           see Rte.h for details \n
 * \b Autosar-Vendor-ID:  41 \n
 *
 * \b Module:             Rte_Type.h \n
 * \b Generator:          Picea Rte V4.9.0-Delivery-Build275 \n
 *
 *      NOTE! This file is generated. Do not edit!
 *
 * \b Changeable-by-user: No \n
 * \b Delivery-File:      No \n
 *
 * \b Module-Owner:       Mecel Picea Team \n
 * \b Location:           Mecel \n
 * \b Phone:              +46 31 720 44 00 \n
 * \b E-Mail:             picea(at)mecel.se \n
 * \b Web:                http://bugzilla.mecel.se/ \n
 *
 * \b Traceability-Info   PICEA* \n
 * \b Classification:     Not classified \n
 * \b Deviations:         See PICEA_RTE_USG_003 \n
 *
 */

/*============================================================================*
 *
 * Copyright 2012 Mecel AB and Delphi Technologies, Inc., All Rights Reserved
 *
 *============================================================================*/
#ifndef RTE_TYPE_H
#define RTE_TYPE_H

/*============================================================================*
 * PREPROCESSOR DIRECTIVES                                                    *
 *============================================================================*/

/* INCLUDE DIRECTIVES FOR OTHER HEADERS --------------------------------------*/

#include "Rte.h"

/* EXPORTED DEFINES FOR CONSTANTS --------------------------------------------*/
#define RTE_TYPE_SW_MAJOR_VERSION (4u)
#define RTE_TYPE_SW_MINOR_VERSION (9u)
#define RTE_TYPE_SW_PATCH_VERSION (0u)

/*============================================================================*
 * EXPORTED TYPEDEF DECLARATIONS                                              *
 *============================================================================*/

/* Implementation data types -------------------------------------------------*/
typedef uint8    EcuM_BootTargetType;
typedef uint8    EcuM_ShutdownCauseType;
typedef uint8    EcuM_StateType;
typedef uint8    EcuM_UserType;
typedef uint16   NvM_BlockIdType;
typedef uint8    imp_target_indDiag[7];

typedef struct
{
   uint8 dt;
} pvU1NoSts;

typedef struct
{
   uint16 dt;
} pvU2NoSts;

typedef struct
{
   uint32 dt;
} pvU4NoSts;

typedef struct
{
   uint8 dt;
   uint8 sts;
} pvU1;

typedef struct
{
   uint16 dt;
   uint8 sts;
} pvU2;

typedef struct
{
   uint32 dt;
   uint8 sts;
} pvU4;

/* Per-Instance-Memory types -------------------------------------------------*/

/* Client-Server types -------------------------------------------------------*/

#endif /* RTE_TYPE_H */
