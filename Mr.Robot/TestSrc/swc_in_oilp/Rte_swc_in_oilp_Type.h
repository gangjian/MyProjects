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
 * \b Module:             Rte_swc_in_oilp_Type.h \n
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
#ifndef RTE_SWC_IN_OILP_TYPE_H
#define RTE_SWC_IN_OILP_TYPE_H

#ifdef __cplusplus
extern "C" {
#endif /* __cplusplus */

/*============================================================================*
 * PREPROCESSOR DIRECTIVES                                                    *
 *============================================================================*/

/* INCLUDE DIRECTIVES FOR OTHER HEADERS --------------------------------------*/

#include "Rte_Type.h"

/* EXPORTED DEFINES FOR CONSTANTS --------------------------------------------*/
#define RTE_SWC_IN_OILP_TYPE_SW_MAJOR_VERSION (4u)
#define RTE_SWC_IN_OILP_TYPE_SW_MINOR_VERSION (9u)
#define RTE_SWC_IN_OILP_TYPE_SW_PATCH_VERSION (0u)

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

#ifndef PV_STS_NORMAL
#define PV_STS_NORMAL ((uint8) 0U)
#endif /* PV_STS_NORMAL */

#ifndef PV_STS_SHORT
#define PV_STS_SHORT ((uint8) 1U)
#endif /* PV_STS_SHORT */

#ifndef PV_STS_TIMEOUT
#define PV_STS_TIMEOUT ((uint8) 2U)
#endif /* PV_STS_TIMEOUT */

#ifndef PV_STS_RNGOVR
#define PV_STS_RNGOVR ((uint8) 3U)
#endif /* PV_STS_RNGOVR */

#ifndef PV_STS_INVALID
#define PV_STS_INVALID ((uint8) 4U)
#endif /* PV_STS_INVALID */

#ifndef PV_STS_JUDGING
#define PV_STS_JUDGING ((uint8) 5U)
#endif /* PV_STS_JUDGING */

#ifndef PV_STS_FAIL
#define PV_STS_FAIL ((uint8) 6U)
#endif /* PV_STS_FAIL */

#ifndef PV_STS_NOTRCV
#define PV_STS_NOTRCV ((uint8) 7U)
#endif /* PV_STS_NOTRCV */

#ifndef PV_STS_ERR
#define PV_STS_ERR ((uint8) 8U)
#endif /* PV_STS_ERR */

#ifndef PV_STS_UNKNOWN
#define PV_STS_UNKNOWN ((uint8) 9U)
#endif /* PV_STS_UNKNOWN */

#ifndef AD_STS_NORMAL
#define AD_STS_NORMAL ((uint8) 0U)
#endif /* AD_STS_NORMAL */

#ifndef AD_STS_ABNORMAL
#define AD_STS_ABNORMAL ((uint8) 1U)
#endif /* AD_STS_ABNORMAL */

#ifndef AD_STS_JUDGING
#define AD_STS_JUDGING ((uint8) 2U)
#endif /* AD_STS_JUDGING */

#ifndef AD_STS_UNKNOWN
#define AD_STS_UNKNOWN ((uint8) 255U)
#endif /* AD_STS_UNKNOWN */

#ifndef IN_NE1_MIN
#define IN_NE1_MIN ((sint16) 0)
#endif /* IN_NE1_MIN */

#ifndef IN_NE1_MAX
#define IN_NE1_MAX ((sint16) 16384)
#endif /* IN_NE1_MAX */

#ifndef IN_NORMALADC_CH_0
#define IN_NORMALADC_CH_0 ((uint32) 0U)
#endif /* IN_NORMALADC_CH_0 */

#ifndef IN_NORMALADC_CH_1
#define IN_NORMALADC_CH_1 ((uint32) 1U)
#endif /* IN_NORMALADC_CH_1 */

#ifndef IN_NORMALADC_CH_2
#define IN_NORMALADC_CH_2 ((uint32) 2U)
#endif /* IN_NORMALADC_CH_2 */

#ifndef IN_NORMALADC_CH_3
#define IN_NORMALADC_CH_3 ((uint32) 3U)
#endif /* IN_NORMALADC_CH_3 */

#ifndef IN_NORMALADC_CH_MAX
#define IN_NORMALADC_CH_MAX ((uint32) 4U)
#endif /* IN_NORMALADC_CH_MAX */

#ifndef IN_NORMALADC_VAL_MIN
#define IN_NORMALADC_VAL_MIN ((uint16) 0U)
#endif /* IN_NORMALADC_VAL_MIN */

#ifndef IN_NORMALADC_VAL_MAX
#define IN_NORMALADC_VAL_MAX ((uint16) 65535U)
#endif /* IN_NORMALADC_VAL_MAX */

#ifndef PV_ENGONOFF3S_OFF
#define PV_ENGONOFF3S_OFF ((uint8) 0U)
#endif /* PV_ENGONOFF3S_OFF */

#ifndef PV_ENGONOFF3S_ON
#define PV_ENGONOFF3S_ON ((uint8) 1U)
#endif /* PV_ENGONOFF3S_ON */

#ifndef PV_OILPAD_MIN
#define PV_OILPAD_MIN ((uint16) 0U)
#endif /* PV_OILPAD_MIN */

#ifndef PV_OILPAD_MAX
#define PV_OILPAD_MAX ((uint16) 1023U)
#endif /* PV_OILPAD_MAX */

#ifndef TIMID_PRC_OILP_OILPAD
#define TIMID_PRC_OILP_OILPAD ((uint16) 0U)
#endif /* TIMID_PRC_OILP_OILPAD */

#ifndef TIMID_IN_OILP_IGAD
#define TIMID_IN_OILP_IGAD ((uint16) 1U)
#endif /* TIMID_IN_OILP_IGAD */

#ifndef TIMID_NUM
#define TIMID_NUM ((uint16) 2U)
#endif /* TIMID_NUM */

#ifndef TM_TIMTYP_ONESHOT
#define TM_TIMTYP_ONESHOT ((uint8) 0U)
#endif /* TM_TIMTYP_ONESHOT */

#ifndef TM_TIMTYP_CYCLIC
#define TM_TIMTYP_CYCLIC ((uint8) 1U)
#endif /* TM_TIMTYP_CYCLIC */

#ifndef TM_TIMTYP_SYNC_ONE
#define TM_TIMTYP_SYNC_ONE ((uint8) 16U)
#endif /* TM_TIMTYP_SYNC_ONE */

#ifndef TM_TIMTYP_SYNC_CYC
#define TM_TIMTYP_SYNC_CYC ((uint8) 17U)
#endif /* TM_TIMTYP_SYNC_CYC */

#ifndef TM_TIMSTS_STOP
#define TM_TIMSTS_STOP ((uint8) 0U)
#endif /* TM_TIMSTS_STOP */

#ifndef TM_TIMSTS_START
#define TM_TIMSTS_START ((uint8) 1U)
#endif /* TM_TIMSTS_START */

#ifndef TM_TIMSTS_TIMEOUT
#define TM_TIMSTS_TIMEOUT ((uint8) 16U)
#endif /* TM_TIMSTS_TIMEOUT */


/* Limits of Range Data Types  -----------------------------------------------*/
#ifndef RTE_CORE

#define common_inMsgSts_LowerLimit ((uint8) 0U)
#define common_inMsgSts_UpperLimit ((uint8) 255U)

#define common_pvSts_LowerLimit ((uint8) 0U)
#define common_pvSts_UpperLimit ((uint8) 255U)

#define common_adSts_LowerLimit ((uint8) 0U)
#define common_adSts_UpperLimit ((uint8) 255U)

#define in_NE1_LowerLimit ((sint16) -32768)
#define in_NE1_UpperLimit ((sint16) 32767)

#define ioHwAb_normalAdcCh_LowerLimit ((uint32) 0U)
#define ioHwAb_normalAdcCh_UpperLimit ((uint32) 4294967295U)

#define ioHwAb_normalAdcVal_LowerLimit ((uint16) 0U)
#define ioHwAb_normalAdcVal_UpperLimit ((uint16) 65535U)

#define pv_PvEngOnOff3s_dt_LowerLimit ((uint8) 0U)
#define pv_PvEngOnOff3s_dt_UpperLimit ((uint8) 255U)

#define pv_PvOilpAd_dt_LowerLimit ((uint16) 0U)
#define pv_PvOilpAd_dt_UpperLimit ((uint16) 65535U)

#define timer_id_LowerLimit ((uint16) 0U)
#define timer_id_UpperLimit ((uint16) 65535U)

#define timer_kind_LowerLimit ((uint8) 0U)
#define timer_kind_UpperLimit ((uint8) 255U)

#define timer_sts_LowerLimit ((uint8) 0U)
#define timer_sts_UpperLimit ((uint8) 255U)

#endif /* RTE_CORE */

#ifdef __cplusplus
} /* extern "C" */
#endif /* __cplusplus */

#endif /* RTE_SWC_IN_OILP_TYPE_H */
