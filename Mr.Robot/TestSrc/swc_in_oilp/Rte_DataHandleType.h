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
 * \b Module:             Rte_DataHandleType.h \n
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
#ifndef RTE_DATAHANDLETYPE_H
#define RTE_DATAHANDLETYPE_H

/*============================================================================*
 * PREPROCESSOR DIRECTIVES                                                    *
 *============================================================================*/

/* INCLUDE DIRECTIVES FOR OTHER HEADERS --------------------------------------*/

/* EXPORTED DEFINES FOR CONSTANTS --------------------------------------------*/
#define RTE_DATAHANDLETYPE_SW_MAJOR_VERSION (4u)
#define RTE_DATAHANDLETYPE_SW_MINOR_VERSION (9u)
#define RTE_DATAHANDLETYPE_SW_PATCH_VERSION (0u)

/*============================================================================*
 * EXPORTED TYPEDEF DECLARATIONS                                              *
 *============================================================================*/

/* Data Handle Types ---------------------------------------------------------*/
typedef struct
{
   pvU1NoSts value;
} Rte_DE_pvU1NoSts;

typedef struct
{
   pvU2NoSts value;
} Rte_DE_pvU2NoSts;

typedef struct
{
   pvU4NoSts value;
} Rte_DE_pvU4NoSts;

/*
typedef struct
{
   pvSts value;
} Rte_DE_pvSts;
*/

typedef struct
{
   pvU1 value;
} Rte_DE_pvU1;

typedef struct
{
   pvU2 value;
} Rte_DE_pvU2;

typedef struct
{
   pvU4 value;
} Rte_DE_pvU4;

typedef struct
{
   uint16 value;
} Rte_DE_uint16;

typedef struct
{
   uint8 value;
} Rte_DE_uint8;

/*	IoHwAbÇ≈íËã`Ç∑ÇÈç\ë¢ëÃÇÃíËã`	*/
typedef struct
{
   uint8 OutputTypePtr;
   uint16 Lumptr;
} IoHwAbPwm_DataCfg;

typedef struct
{
   IoHwAbPwm_DataCfg value;
} Rte_DE_IoHwAbPwm_DataCfg;

typedef struct
{
   sint16 value;
} Rte_DE_sint16;
#endif /* RTE_DATAHANDLETYPE_H */
