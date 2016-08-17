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
 * \b Module:             Rte_swc_in_trcta_Type.h \n
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
#ifndef RTE_SWC_IN_TRCTA_TYPE_H
#define RTE_SWC_IN_TRCTA_TYPE_H

#ifdef __cplusplus
extern "C" {
#endif /* __cplusplus */

/*============================================================================*
 * PREPROCESSOR DIRECTIVES                                                    *
 *============================================================================*/

/* INCLUDE DIRECTIVES FOR OTHER HEADERS --------------------------------------*/

#include "Rte_Type.h"

/* EXPORTED DEFINES FOR CONSTANTS --------------------------------------------*/
#define RTE_SWC_IN_TRCTA_TYPE_SW_MAJOR_VERSION (4u)
#define RTE_SWC_IN_TRCTA_TYPE_SW_MINOR_VERSION (9u)
#define RTE_SWC_IN_TRCTA_TYPE_SW_PATCH_VERSION (0u)

/*============================================================================*
 * EXPORTED TYPEDEF DECLARATIONS                                              *
 *============================================================================*/

/* Mode Declaration Groups ---------------------------------------------------*/
#ifndef RTE_MODETYPE_IG
#define RTE_MODETYPE_IG
typedef uint8 Rte_ModeType_IG;
#endif
#ifndef RTE_MODE_IG_OFF
#define RTE_MODE_IG_OFF 0U
#endif
#ifndef RTE_MODE_IG_ON
#define RTE_MODE_IG_ON 1U
#endif
#ifndef RTE_MODE_IG_UNKNOWN
#define RTE_MODE_IG_UNKNOWN 2U
#endif
#ifndef RTE_TRANSITION_IG
#define RTE_TRANSITION_IG 3U
#endif


/* Enumeration Data Types ----------------------------------------------------*/
#ifndef IN_MSGSTS_NONE
#define IN_MSGSTS_NONE ((uint8) 0U)
#endif /* IN_MSGSTS_NONE */

#ifndef IN_MSGSTS_NG
#define IN_MSGSTS_NG ((uint8) 128U)
#endif /* IN_MSGSTS_NG */

#ifndef IN_MSGSTS_FAILINIT
#define IN_MSGSTS_FAILINIT ((uint8) 32U)
#endif /* IN_MSGSTS_FAILINIT */

#ifndef IN_MSGSTS_TXSTOP
#define IN_MSGSTS_TXSTOP ((uint8) 16U)
#endif /* IN_MSGSTS_TXSTOP */

#ifndef IN_MSGSTS_TIMEOUT
#define IN_MSGSTS_TIMEOUT ((uint8) 2U)
#endif /* IN_MSGSTS_TIMEOUT */

#ifndef IN_MSGSTS_NORX
#define IN_MSGSTS_NORX ((uint8) 1U)
#endif /* IN_MSGSTS_NORX */

#ifndef IN_TRCTA_OFF
#define IN_TRCTA_OFF ((uint8) 0U)
#endif /* IN_TRCTA_OFF */

#ifndef IN_TRCTA_ON
#define IN_TRCTA_ON ((uint8) 1U)
#endif /* IN_TRCTA_ON */

#ifndef PV_RCTASW_IN_OFF
#define PV_RCTASW_IN_OFF ((uint8) 0U)
#endif /* PV_RCTASW_IN_OFF */

#ifndef PV_RCTASW_IN_ON
#define PV_RCTASW_IN_ON ((uint8) 1U)
#endif /* PV_RCTASW_IN_ON */

#ifndef PV_RCTASW_IN_UNFIX
#define PV_RCTASW_IN_UNFIX ((uint8) 2U)
#endif /* PV_RCTASW_IN_UNFIX */


/* Limits of Range Data Types  -----------------------------------------------*/
#ifndef RTE_CORE

#define common_inMsgSts_LowerLimit ((uint8) 0U)
#define common_inMsgSts_UpperLimit ((uint8) 255U)

#define in_TRCTA_LowerLimit ((uint8) 0U)
#define in_TRCTA_UpperLimit ((uint8) 255U)

#define pv_PvRctasw_dt_LowerLimit ((uint8) 0U)
#define pv_PvRctasw_dt_UpperLimit ((uint8) 255U)

#endif /* RTE_CORE */

#ifdef __cplusplus
} /* extern "C" */
#endif /* __cplusplus */

#endif /* RTE_SWC_IN_TRCTA_TYPE_H */
