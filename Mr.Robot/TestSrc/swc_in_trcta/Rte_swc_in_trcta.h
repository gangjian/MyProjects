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
 * \b Module:             Rte_swc_in_trcta.h \n
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
#ifndef RTE_SWC_IN_TRCTA_H
#define RTE_SWC_IN_TRCTA_H

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

#include "Rte_swc_in_trcta_Type.h"
#include "Rte_DataHandleType.h"

/* EXPORTED DEFINES FOR CONSTANTS --------------------------------------------*/
#define RTE_SWC_IN_TRCTA_SW_MAJOR_VERSION (4u)
#define RTE_SWC_IN_TRCTA_SW_MINOR_VERSION (9u)
#define RTE_SWC_IN_TRCTA_SW_PATCH_VERSION (0u)

#ifndef RTE_CORE
/* Application errors --------------------------------------------------------*/

/* Init values ---------------------------------------------------------------*/

/* API Mapping ---------------------------------------------------------------*/
#define Rte_IWrite_rbl_in_trcta_igoff_pp_srIf_pv_PvRctasw_struct(data) ((Rte_Inst_swc_in_trcta->rbl_in_trcta_igoff_pp_srIf_pv_PvRctasw_struct)->value = (*data))
#define Rte_IWriteRef_rbl_in_trcta_igoff_pp_srIf_pv_PvRctasw_struct() (&((Rte_Inst_swc_in_trcta->rbl_in_trcta_igoff_pp_srIf_pv_PvRctasw_struct)->value))
#define Rte_IRead_rbl_in_trcta_igon_rp_srIf_in_TRCTA_val() ((*(Rte_Inst_swc_in_trcta->rbl_in_trcta_igon_rp_srIf_in_TRCTA_val)).value)
#define Rte_IRead_rbl_in_trcta_igon_rp_srIf_in_TRCTASts_val() ((*(Rte_Inst_swc_in_trcta->rbl_in_trcta_igon_rp_srIf_in_TRCTASts_val)).value)
#define Rte_IWrite_rbl_in_trcta_igon_pp_srIf_pv_PvRctasw_struct(data) ((Rte_Inst_swc_in_trcta->rbl_in_trcta_igon_pp_srIf_pv_PvRctasw_struct)->value = (*data))
#define Rte_IWriteRef_rbl_in_trcta_igon_pp_srIf_pv_PvRctasw_struct() (&((Rte_Inst_swc_in_trcta->rbl_in_trcta_igon_pp_srIf_pv_PvRctasw_struct)->value))
#define Rte_IWrite_rbl_in_trcta_initReset_pp_srIf_pv_PvRctasw_struct(data) ((Rte_Inst_swc_in_trcta->rbl_in_trcta_initReset_pp_srIf_pv_PvRctasw_struct)->value = (*data))
#define Rte_IWriteRef_rbl_in_trcta_initReset_pp_srIf_pv_PvRctasw_struct() (&((Rte_Inst_swc_in_trcta->rbl_in_trcta_initReset_pp_srIf_pv_PvRctasw_struct)->value))
#define Rte_IWrite_rbl_in_trcta_initWakeup_pp_srIf_pv_PvRctasw_struct(data) ((Rte_Inst_swc_in_trcta->rbl_in_trcta_initWakeup_pp_srIf_pv_PvRctasw_struct)->value = (*data))
#define Rte_IWriteRef_rbl_in_trcta_initWakeup_pp_srIf_pv_PvRctasw_struct() (&((Rte_Inst_swc_in_trcta->rbl_in_trcta_initWakeup_pp_srIf_pv_PvRctasw_struct)->value))

/* Port handle API Mapping ---------------------------------------------------*/
#endif

/*============================================================================*
 * EXPORTED TYPEDEF DECLARATIONS                                              *
 *============================================================================*/

/* PDS/CDS local data types --------------------------------------------------*/

/* Port Data Structures (PDS) ------------------------------------------------*/

/* Component Data Structure (CDS) --------------------------------------------*/
struct Rte_CDS_swc_in_trcta
{
   /* Data Handles section. -----------------------*/
   CONSTP2VAR(Rte_DE_pvU1NoSts, RTE_CONST, RTE_APPL_DATA) rbl_in_trcta_igoff_pp_srIf_pv_PvRctasw_struct;
   CONSTP2VAR(Rte_DE_pvU1NoSts, RTE_CONST, RTE_APPL_DATA) rbl_in_trcta_igon_pp_srIf_pv_PvRctasw_struct;
   CONSTP2VAR(Rte_DE_uint8, RTE_CONST, RTE_APPL_DATA) rbl_in_trcta_igon_rp_srIf_in_TRCTA_val;
   CONSTP2VAR(Rte_DE_uint8, RTE_CONST, RTE_APPL_DATA) rbl_in_trcta_igon_rp_srIf_in_TRCTASts_val;
   CONSTP2VAR(Rte_DE_pvU1NoSts, RTE_CONST, RTE_APPL_DATA) rbl_in_trcta_initReset_pp_srIf_pv_PvRctasw_struct;
   CONSTP2VAR(Rte_DE_pvU1NoSts, RTE_CONST, RTE_APPL_DATA) rbl_in_trcta_initWakeup_pp_srIf_pv_PvRctasw_struct;
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
typedef P2CONST(struct Rte_CDS_swc_in_trcta, TYPEDEF, RTE_CONST) Rte_Instance;
#endif

/*============================================================================*
 * EXPORTED OBJECT DECLARATIONS                                               *
 *============================================================================*/

#define RTE_START_SEC_CONST_UNSPECIFIED
#include "MemMap.h"

extern CONSTP2CONST(struct Rte_CDS_swc_in_trcta, RTE_CONST, RTE_APPL_CONST) Rte_Inst_swc_in_trcta;

#define RTE_STOP_SEC_CONST_UNSPECIFIED
#include "MemMap.h"

/*============================================================================*
 * EXPORTED FUNCTIONS PROTOTYPES                                              *
 *============================================================================*/
/* Declaration of runnable entities ------------------------------------------*/

#define swc_in_trcta_START_SEC_CODE
#include "swc_in_trcta_MemMap.h"

extern FUNC(void, swc_in_trcta_CODE) sym_rbl_in_trcta_igoff(void);
extern FUNC(void, swc_in_trcta_CODE) sym_rbl_in_trcta_igon(void);

#define swc_in_trcta_STOP_SEC_CODE
#include "swc_in_trcta_MemMap.h"


#define RTE_START_SEC_CODE
#include "MemMap.h"

/* Declaration of API functions ----------------------------------------------*/

#define RTE_STOP_SEC_CODE
#include "MemMap.h"

/* PRQA S 777 -- */

#ifdef __cplusplus
} /* extern "C" */
#endif /* __cplusplus */

#endif /* RTE_SWC_IN_TRCTA_H */
