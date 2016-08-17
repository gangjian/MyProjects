/*	$RCSfile: Rte_swc_in_trcta.c $									*/
/*	$Date: 2015/11/05 16:43:06JST $									*/
/*	$Revision: 1.2 $												*/
/*	 EXPLANATION: [RCTA(テルテール)](入力) ソースファイル			*/
/** 
 * \file Rte_swc_in_trcta.c
 *
 * \brief <Add a description here>
 *
 * \b Generator: Picea Rte V4.9.0-Delivery-Build275
 */

/*============================================================================*
 * PREPROCESSOR DIRECTIVES                                                    *
 *============================================================================*/

/* INCLUDE DIRECTIVES FOR OTHER HEADERS --------------------------------------*/

#include "Rte_swc_in_trcta.h"

/*============================================================================*
 * PREPROCESSOR DIRECTIVES                                                    *
 *============================================================================*/

/* INCLUDE DIRECTIVES FOR OTHER HEADERS --------------------------------------*/

#include "share_lib_can_in_step.h"			/* CAN標準入力(step)共通ライブラリ */

/* EXPORTED DEFINES FOR CONSTANTS --------------------------------------------*/

/*-----------------------------------------------------------------------------/
 *
 * Used Enumeration Data types
 * - cmp_common_inMsgSts (uint8)
 *   - IN_MSGSTS_NONE
 *   - IN_MSGSTS_NG
 *   - IN_MSGSTS_FAILINIT
 *   - IN_MSGSTS_TXSTOP
 *   - IN_MSGSTS_TIMEOUT
 *   - IN_MSGSTS_NORX
 * - cmp_in_TRCTA (uint8)
 *   - IN_TRCTA_OFF
 *   - IN_TRCTA_ON
 * - cmp_pv_PvRctasw (uint8)
 *   - PV_RCTASW_IN_OFF
 *   - PV_RCTASW_IN_ON
 *   - PV_RCTASW_IN_UNFIX
 *
 *----------------------------------------------------------------------------*/
/*	出力値テーブルのサイズ	*/
#define IN_RCTASW_TBL_SIZE			((uint8) 3)

/*============================================================================*
 * EXPORTED TYPEDEF DECLARATIONS                                              *
 *============================================================================*/

/*-----------------------------------------------------------------------------/
 *
 * Used Implementation Data Types
 *  uint8
 *  uint8
 *  pvU1NoSts: Record with 1 fields.
 *
 *----------------------------------------------------------------------------*/

/*============================================================================*
 * EXPORTED OBJECT DECLARATIONS                                               *
 *============================================================================*/
/* Port interfaces -----------------------------------------------------------*/

/* Per-Instance-Memorys ------------------------------------------------------*/

/* Calibration Datas ----------------------------------------------------------*/

/* Calibration Parameters ----------------------------------------------------*/

/*============================================================================*
 * STATIC PARAMETERS		                                                  *
 *============================================================================*/
#define swc_in_trcta_START_SEC_CONST_UNSPECIFIED
#include "swc_in_trcta_MemMap.h"

/*	出力値テーブル	*/
/*	RCTA_SW 出力値テーブル	*/
static const uint8 inTblRctasw[IN_RCTASW_TBL_SIZE] =
{
	PV_RCTASW_IN_OFF,
	PV_RCTASW_IN_ON,
	PV_RCTASW_IN_UNFIX
};

/*	入力共通処理(VAL)マネージャテーブル	*/
static const MNG_STEP_VAL rctasw_can_mng_tbl = {
	IN_RCTASW_TBL_SIZE,								/*	入力信号最大値			*/
	PV_RCTASW_IN_OFF,								/*	物理値初期値			*/
	PV_RCTASW_IN_OFF,								/*	途絶時物理値			*/
	PV_RCTASW_IN_ON,								/*	範囲外時物理値			*/
	&inTblRctasw[0]									/*	変換テーブルアドレス	*/
};

#define swc_in_trcta_STOP_SEC_CONST_UNSPECIFIED
#include "swc_in_trcta_MemMap.h"

/*============================================================================*
 * EXPORTED FUNCTIONS PROTOTYPES                                              *
 *============================================================================*/
/* Declaration of runnable entities ------------------------------------------*/

/*-----------------------------------------------------------------------------/
 *
 * Runnable Entity: sym_rbl_in_trcta_igoff
 *  Can be invoked concurrently: TRUE
 *  Events:
 *  - Triggerred by ev_te_in_trcta_igoff (TIMING-EVENT) once every 0.01 seconds
 *    but not in mode(s): ModeDeclGroup_ig
 *
 *-----------------------------------------------------------------------------/
 *
 * Implicit Sender-Reciever Data-Write-Access API(s): 
 * - void Rte_IWrite_rbl_in_trcta_igoff_pp_srIf_pv_PvRctasw_struct(const pvU1NoSts* struct)
 * - pvU1NoSts* Rte_IWriteRef_rbl_in_trcta_igoff_pp_srIf_pv_PvRctasw_struct(void)
 *
 *----------------------------------------------------------------------------*/

#define swc_in_trcta_START_SEC_CODE
#include "swc_in_trcta_MemMap.h"

/********************************************************************************/
/*	関数概要：RCTA(テルテール)(入力) IG-OFF処理									*/
/*	処理時期：																	*/
/*																				*/
/*	引数	：無し																*/
/*	戻り値	：無し																*/
/*	注意事項：無し																*/
/********************************************************************************/
FUNC(void, swc_in_trcta_CODE) sym_rbl_in_trcta_igoff(void)
{
   /* Insert your code here! */
	pvU1NoSts	pvOut;									/*	物理値				*/

	/*	物理値初期化(データ)	*/
	pvOut.dt = PV_RCTASW_IN_OFF;
	Rte_IWrite_rbl_in_trcta_igoff_pp_srIf_pv_PvRctasw_struct(&pvOut);
}

#define swc_in_trcta_STOP_SEC_CODE
#include "swc_in_trcta_MemMap.h"


/*-----------------------------------------------------------------------------/
 *
 * Runnable Entity: sym_rbl_in_trcta_igon
 *  Can be invoked concurrently: TRUE
 *  Events:
 *  - Triggerred by ev_te_in_trcta_igon (TIMING-EVENT) once every 0.01 seconds
 *    but not in mode(s): ModeDeclGroup_ig, ModeDeclGroup_ig
 *
 *-----------------------------------------------------------------------------/
 *
 * Implicit Sender-Reciever Data-Read-Access API(s): 
 * - uint8 Rte_IRead_rbl_in_trcta_igon_rp_srIf_in_TRCTA_val(void)
 * - uint8 Rte_IRead_rbl_in_trcta_igon_rp_srIf_in_TRCTASts_val(void)
 *
 * Implicit Sender-Reciever Data-Write-Access API(s): 
 * - void Rte_IWrite_rbl_in_trcta_igon_pp_srIf_pv_PvRctasw_struct(const pvU1NoSts* struct)
 * - pvU1NoSts* Rte_IWriteRef_rbl_in_trcta_igon_pp_srIf_pv_PvRctasw_struct(void)
 *
 *----------------------------------------------------------------------------*/

#define swc_in_trcta_START_SEC_CODE
#include "swc_in_trcta_MemMap.h"

/********************************************************************************/
/*	関数概要：RCTA(テルテール)(入力) IG-ON処理									*/
/*	処理時期：																	*/
/*																				*/
/*	引数	：無し																*/
/*	戻り値	：無し																*/
/*	注意事項：無し																*/
/********************************************************************************/
FUNC(void, swc_in_trcta_CODE) sym_rbl_in_trcta_igon(void)
{
   /* Insert your code here! */
	VAR_IN_STEP		varInStep;							/*	入力テーブル		*/
	VAR_OUT_STEP	varOutStep;							/*	出力テーブル		*/
	pvU1NoSts		pvOut;								/*	物理値				*/

	/*	物理値の初期化	*/
	varOutStep.pv = PV_RCTASW_IN_OFF;

	/*	入力値→物理値変換	*/
	varInStep.inSig = Rte_IRead_rbl_in_trcta_igon_rp_srIf_in_TRCTA_val();
	varInStep.msgSts = Rte_IRead_rbl_in_trcta_igon_rp_srIf_in_TRCTASts_val();
	varInStep.powerSts = SHARE_LIB_POWER_ON;
	ShareLibStepFailJudgeVal(&varInStep, &rctasw_can_mng_tbl ,&varOutStep);

	/*	物理値出力	*/
	pvOut.dt = varOutStep.pv;
	Rte_IWrite_rbl_in_trcta_igon_pp_srIf_pv_PvRctasw_struct(&pvOut);
}

#define swc_in_trcta_STOP_SEC_CODE
#include "swc_in_trcta_MemMap.h"


#define swc_in_trcta_START_SEC_CODE
#include "swc_in_trcta_MemMap.h"

/********************************************************************************/
/*	関数概要：RCTA(テルテール)(入力) 初期化処理(リセット)						*/
/*	処理時期：																	*/
/*																				*/
/*	引数	：無し																*/
/*	戻り値	：無し																*/
/*	注意事項：無し																*/
/********************************************************************************/
FUNC(void, swc_in_trcta_CODE) sym_rbl_in_trcta_initReset(void)
{
   /* Insert your code here! */
	pvU1NoSts	pvOut;									/*	物理値				*/

	/*	物理値初期化(データ)	*/
	pvOut.dt = PV_RCTASW_IN_OFF;
	Rte_IWrite_rbl_in_trcta_initReset_pp_srIf_pv_PvRctasw_struct(&pvOut);
}

#define swc_in_trcta_STOP_SEC_CODE
#include "swc_in_trcta_MemMap.h"


#define swc_in_trcta_START_SEC_CODE
#include "swc_in_trcta_MemMap.h"

/********************************************************************************/
/*	関数概要：RCTA(テルテール)(入力) 初期化処理(ウェイクアップ)					*/
/*	処理時期：																	*/
/*																				*/
/*	引数	：無し																*/
/*	戻り値	：無し																*/
/*	注意事項：無し																*/
/********************************************************************************/
FUNC(void, swc_in_trcta_CODE) sym_rbl_in_trcta_initWakeup(void)
{
   /* Insert your code here! */
	pvU1NoSts	pvOut;									/*	物理値				*/

	/*	物理値初期化(データ)	*/
	pvOut.dt = PV_RCTASW_IN_OFF;
	Rte_IWrite_rbl_in_trcta_initWakeup_pp_srIf_pv_PvRctasw_struct(&pvOut);
}

#define swc_in_trcta_STOP_SEC_CODE
#include "swc_in_trcta_MemMap.h"


