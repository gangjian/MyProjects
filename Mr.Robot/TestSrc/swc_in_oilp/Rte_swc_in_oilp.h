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
 * \b Module:             Rte_swc_in_oilp.h \n
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
#ifndef RTE_SWC_IN_OILP_H
#define RTE_SWC_IN_OILP_H

#ifndef RTE_CORE
#ifdef RTE_APPLICATION_HEADER_FILE
#error Multiple application header files included.
#endif /* RTE_APPLICATION_HEADER_FILE */
#define RTE_APPLICATION_HEADER_FILE
#endif /* RTE_CORE */

#ifdef __cplusplus
extern "C" {
#endif /* __cplusplus */

/*============================================================================*
 * PREPROCESSOR DIRECTIVES                                                    *
 *============================================================================*/
/* PRQA S 777 ++
   Variable names are (partly) defined by user in SWC configuration.
*/

/* INCLUDE DIRECTIVES FOR OTHER HEADERS --------------------------------------*/

#include "Rte_swc_in_oilp_Type.h"
#include "Rte_DataHandleType.h"

/* EXPORTED DEFINES FOR CONSTANTS --------------------------------------------*/
#define RTE_SWC_IN_OILP_SW_MAJOR_VERSION (4u)
#define RTE_SWC_IN_OILP_SW_MINOR_VERSION (9u)
#define RTE_SWC_IN_OILP_SW_PATCH_VERSION (0u)

#ifndef RTE_CORE
/* Application errors --------------------------------------------------------*/

/* Init values ---------------------------------------------------------------*/

/* API Mapping ---------------------------------------------------------------*/
#define Rte_Call_rp_csIf_timer_startTimer_op Rte_Call_swc_in_oilp_rp_csIf_timer_startTimer_op
#define Rte_Call_rp_csIf_timer_sts_op Rte_Call_swc_in_oilp_rp_csIf_timer_sts_op
#define Rte_Call_rp_csIf_ioHwAb_normalAdcStsGet_op Rte_Call_swc_in_oilp_rp_csIf_ioHwAb_normalAdcStsGet_op
#define Rte_Call_rp_csIf_ioHwAb_normalAdcValGet_op Rte_Call_swc_in_oilp_rp_csIf_ioHwAb_normalAdcValGet_op
#define Rte_IRead_rbl_in_oilp_rp_srIf_common_igOnTime_val() ((*(Rte_Inst_swc_in_oilp->rbl_in_oilp_rp_srIf_common_igOnTime_val)).value)
#define Rte_IRead_rbl_in_oilp_rp_srIf_in_NE1_val() ((*(Rte_Inst_swc_in_oilp->rbl_in_oilp_rp_srIf_in_NE1_val)).value)
#define Rte_IRead_rbl_in_oilp_rp_srIf_in_NE1Sts_val() ((*(Rte_Inst_swc_in_oilp->rbl_in_oilp_rp_srIf_in_NE1Sts_val)).value)
#define Rte_IWrite_rbl_in_oilp_pp_srIf_pv_PvEngOnOff3s_struct(data) ((Rte_Inst_swc_in_oilp->rbl_in_oilp_pp_srIf_pv_PvEngOnOff3s_struct)->value = (*data))
#define Rte_IWriteRef_rbl_in_oilp_pp_srIf_pv_PvEngOnOff3s_struct() (&((Rte_Inst_swc_in_oilp->rbl_in_oilp_pp_srIf_pv_PvEngOnOff3s_struct)->value))
#define Rte_IWrite_rbl_in_oilp_pp_srIf_pv_PvOilpAd_struct(data) ((Rte_Inst_swc_in_oilp->rbl_in_oilp_pp_srIf_pv_PvOilpAd_struct)->value = (*data))
#define Rte_IWriteRef_rbl_in_oilp_pp_srIf_pv_PvOilpAd_struct() (&((Rte_Inst_swc_in_oilp->rbl_in_oilp_pp_srIf_pv_PvOilpAd_struct)->value))
#define Rte_IWrite_rbl_in_oilp_initReset_pp_srIf_pv_PvEngOnOff3s_struct(data) ((Rte_Inst_swc_in_oilp->rbl_in_oilp_initReset_pp_srIf_pv_PvEngOnOff3s_struct)->value = (*data))
#define Rte_IWriteRef_rbl_in_oilp_initReset_pp_srIf_pv_PvEngOnOff3s_struct() (&((Rte_Inst_swc_in_oilp->rbl_in_oilp_initReset_pp_srIf_pv_PvEngOnOff3s_struct)->value))
#define Rte_IWrite_rbl_in_oilp_initReset_pp_srIf_pv_PvOilpAd_struct(data) ((Rte_Inst_swc_in_oilp->rbl_in_oilp_initReset_pp_srIf_pv_PvOilpAd_struct)->value = (*data))
#define Rte_IWriteRef_rbl_in_oilp_initReset_pp_srIf_pv_PvOilpAd_struct() (&((Rte_Inst_swc_in_oilp->rbl_in_oilp_initReset_pp_srIf_pv_PvOilpAd_struct)->value))
#define Rte_IWrite_rbl_in_oilp_initWakeup_pp_srIf_pv_PvEngOnOff3s_struct(data) ((Rte_Inst_swc_in_oilp->rbl_in_oilp_initWakeup_pp_srIf_pv_PvEngOnOff3s_struct)->value = (*data))
#define Rte_IWriteRef_rbl_in_oilp_initWakeup_pp_srIf_pv_PvEngOnOff3s_struct() (&((Rte_Inst_swc_in_oilp->rbl_in_oilp_initWakeup_pp_srIf_pv_PvEngOnOff3s_struct)->value))
#define Rte_IWrite_rbl_in_oilp_initWakeup_pp_srIf_pv_PvOilpAd_struct(data) ((Rte_Inst_swc_in_oilp->rbl_in_oilp_initWakeup_pp_srIf_pv_PvOilpAd_struct)->value = (*data))
#define Rte_IWriteRef_rbl_in_oilp_initWakeup_pp_srIf_pv_PvOilpAd_struct() (&((Rte_Inst_swc_in_oilp->rbl_in_oilp_initWakeup_pp_srIf_pv_PvOilpAd_struct)->value))
#define Rte_Mode_rp_msIf_ig_ModeDeclGroup_ig Rte_Mode_swc_in_oilp_rp_msIf_ig_ModeDeclGroup_ig

/* Port handle API Mapping ---------------------------------------------------*/
#endif

/*============================================================================*
 * EXPORTED TYPEDEF DECLARATIONS                                              *
 *============================================================================*/

/* PDS/CDS local data types --------------------------------------------------*/

/* Port Data Structures (PDS) ------------------------------------------------*/

/* Component Data Structure (CDS) --------------------------------------------*/
struct Rte_CDS_swc_in_oilp
{
   /* Data Handles section. -----------------------*/
   CONSTP2VAR(Rte_DE_pvU1, RTE_CONST, RTE_APPL_DATA) rbl_in_oilp_pp_srIf_pv_PvEngOnOff3s_struct;
   CONSTP2VAR(Rte_DE_pvU2, RTE_CONST, RTE_APPL_DATA) rbl_in_oilp_pp_srIf_pv_PvOilpAd_struct;
   CONSTP2VAR(Rte_DE_uint16, RTE_CONST, RTE_APPL_DATA) rbl_in_oilp_rp_srIf_common_igOnTime_val;
   CONSTP2VAR(Rte_DE_sint16, RTE_CONST, RTE_APPL_DATA) rbl_in_oilp_rp_srIf_in_NE1_val;
   CONSTP2VAR(Rte_DE_uint8, RTE_CONST, RTE_APPL_DATA) rbl_in_oilp_rp_srIf_in_NE1Sts_val;
   CONSTP2VAR(Rte_DE_pvU1, RTE_CONST, RTE_APPL_DATA) rbl_in_oilp_initReset_pp_srIf_pv_PvEngOnOff3s_struct;
   CONSTP2VAR(Rte_DE_pvU2, RTE_CONST, RTE_APPL_DATA) rbl_in_oilp_initReset_pp_srIf_pv_PvOilpAd_struct;
   CONSTP2VAR(Rte_DE_pvU1, RTE_CONST, RTE_APPL_DATA) rbl_in_oilp_initWakeup_pp_srIf_pv_PvEngOnOff3s_struct;
   CONSTP2VAR(Rte_DE_pvU2, RTE_CONST, RTE_APPL_DATA) rbl_in_oilp_initWakeup_pp_srIf_pv_PvOilpAd_struct;
   /* Per-instance Memory Handles section. --------*/
   /* Inter-runnable Variable Handles section. ----*/
   /* Calibration Parameter Handles section. ------*/
   /* Exclusive-area API section. -----------------*/
   /* Port API section. ---------------------------*/
   /* Inter Runnable Variable API section. --------*/
   /* Inter Runnable Triggering API section. ------*/
   /* Vendor specific section. --------------------*/
};

#ifndef RTE_CORE
/* Port handle types ---------------------------------------------------------*/

/* Pim types -----------------------------------------------------------------*/

/* Instance handle type ------------------------------------------------------*/
typedef P2CONST(struct Rte_CDS_swc_in_oilp, TYPEDEF, RTE_CONST) Rte_Instance;
#endif

/*============================================================================*
 * EXPORTED OBJECT DECLARATIONS                                               *
 *============================================================================*/

#define RTE_START_SEC_CONST_UNSPECIFIED
#include "MemMap.h"

extern CONSTP2CONST(struct Rte_CDS_swc_in_oilp, RTE_CONST, RTE_APPL_CONST) Rte_Inst_swc_in_oilp;

#define RTE_STOP_SEC_CONST_UNSPECIFIED
#include "MemMap.h"

/*============================================================================*
 * EXPORTED FUNCTIONS PROTOTYPES                                              *
 *============================================================================*/
/* Declaration of runnable entities ------------------------------------------*/

#define swc_in_oilp_START_SEC_CODE
#include "swc_in_oilp_MemMap.h"

extern FUNC(void, swc_in_oilp_CODE) sym_rbl_in_oilp(void);

#define swc_in_oilp_STOP_SEC_CODE
#include "swc_in_oilp_MemMap.h"


#define RTE_START_SEC_CODE
#include "MemMap.h"

/* Declaration of API functions ----------------------------------------------*/
extern FUNC(Std_ReturnType, RTE_CODE) Rte_Call_swc_in_oilp_rp_csIf_timer_startTimer_op(uint16 id, uint8 type, uint16 period);
extern FUNC(Std_ReturnType, RTE_CODE) Rte_Call_swc_in_oilp_rp_csIf_timer_sts_op(uint16 id, P2VAR(uint8, AUTOMATIC, RTE_APPL_DATA) sts);
extern FUNC(Std_ReturnType, RTE_CODE) Rte_Call_swc_in_oilp_rp_csIf_ioHwAb_normalAdcStsGet_op(uint32 id, P2VAR(uint8, AUTOMATIC, RTE_APPL_DATA) sts);
extern FUNC(Std_ReturnType, RTE_CODE) Rte_Call_swc_in_oilp_rp_csIf_ioHwAb_normalAdcValGet_op(uint32 id, P2VAR(uint16, AUTOMATIC, RTE_APPL_DATA) val);
extern FUNC(uint8, RTE_CODE) Rte_Mode_swc_in_oilp_rp_msIf_ig_ModeDeclGroup_ig(void);

#define RTE_STOP_SEC_CODE
#include "MemMap.h"

/* PRQA S 777 -- */

#ifdef __cplusplus
} /* extern "C" */
#endif /* __cplusplus */

#endif /* RTE_SWC_IN_OILP_H */
