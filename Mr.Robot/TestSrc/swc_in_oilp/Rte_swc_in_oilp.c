/*	$RCSfile: Rte_swc_in_oilp.c $										*/
/*	$Date: 2015/11/26 20:56:41JST $										*/
/*	$Revision: 1.2 $													*/
/*	 EXPLANATION: オイルプレッシャゲージ(指針表示)(入力) ソースファイル	*/

/**
 * \file Rte_swc_in_oilp_template.c
 *
 * \brief <Add a description here>
 *
 * \b Generator: Picea Rte V4.9.0-Delivery-Build275
 */

/*============================================================================*
 * PREPROCESSOR DIRECTIVES                                                    *
 *============================================================================*/

/* INCLUDE DIRECTIVES FOR OTHER HEADERS --------------------------------------*/

#include "Rte_swc_in_oilp.h"

/*============================================================================*
 * PREPROCESSOR DIRECTIVES                                                    *
 *============================================================================*/

/* INCLUDE DIRECTIVES FOR OTHER HEADERS --------------------------------------*/

#include "bsw_common.h"							/* BSW共通ヘッダファイル														*/
#include "Rte_swc_in_oilp_map.h"				/* オイルプレッシャゲージ(指針表示)(入力) コンフィギュレーションヘッダファイル	*/
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
 * - cmp_common_pvSts (uint8)
 *   - PV_STS_NORMAL
 *   - PV_STS_SHORT
 *   - PV_STS_TIMEOUT
 *   - PV_STS_RNGOVR
 *   - PV_STS_INVALID
 *   - PV_STS_JUDGING
 *   - PV_STS_FAIL
 *   - PV_STS_NOTRCV
 *   - PV_STS_ERR
 *   - PV_STS_UNKNOWN
 * - cmp_common_adSts (uint8)
 *   - AD_STS_NORMAL
 *   - AD_STS_ABNORMAL
 *   - AD_STS_JUDGING
 *   - AD_STS_UNKNOWN
 * - cmp_in_NE1 (sint16)
 *   - IN_NE1_MIN
 *   - IN_NE1_MAX
 * - cmp_ioHwAb_normalAdcCh (uint32)
 *   - IN_NORMALADC_CH_0
 *   - IN_NORMALADC_CH_1
 *   - IN_NORMALADC_CH_2
 *   - IN_NORMALADC_CH_3
 *   - IN_NORMALADC_CH_MAX
 * - cmp_ioHwAb_normalAdcVal (uint16)
 *   - IN_NORMALADC_VAL_MIN
 *   - IN_NORMALADC_VAL_MAX
 * - cmp_pv_PvEngOnOff3s (uint8)
 *   - PV_ENGONOFF3S_OFF
 *   - PV_ENGONOFF3S_ON
 * - cmp_pv_PvOilpAd (uint16)
 *   - PV_OILPAD_MIN
 *   - PV_OILPAD_MAX
 * - cmp_timer_id (uint16)
 *   - TIMID_PRC_OILP_OILPAD
 *   - TIMID_IN_OILP_IGAD
 *   - TIMID_NUM
 * - cmp_timer_kind (uint8)
 *   - TM_TIMTYP_ONESHOT
 *   - TM_TIMTYP_CYCLIC
 *   - TM_TIMTYP_SYNC_ONE
 *   - TM_TIMTYP_SYNC_CYC
 * - cmp_timer_sts (uint8)
 *   - TM_TIMSTS_STOP
 *   - TM_TIMSTS_START
 *   - TM_TIMSTS_TIMEOUT
 *
 *----------------------------------------------------------------------------*/

/* エンジン回転数 */
#define NE1SIGNED_IN_DATA_MIN			((uint16) 0x0000)					/* エンジン回転数signedの最小値 (LSB:0.78125(rpm))	*/

#define ENGREV3S_IN_MIN					((uint16) 0x0000)					/* エンジン回転数の最小値 (LSB:0.78125(rpm))		*/
#define ENGREV3S_IN_MAX					((uint16) 0x4000)					/* エンジン回転数の最大値 (LSB:0.78125(rpm))		*/
#define ENGREV3S_IN_STS_NORMAL			((uint8) 0x00)						/* 正常												*/
#define ENGREV3S_IN_STS_FAIL			((uint8) 0x01)						/* フェール中										*/
#define ENGREV3S_IN_STS_UNFIX			((uint8) 0x02)						/* 未確定											*/

/* IG電圧A/D値 */
#define IGV_IN_IG_AD_MIN				((uint16)0)							/* IG電圧A/D値：最小値								*/

/* IG電圧A/D値データ状態 */
#define IGV_IN_AD_ST_INACTIVE			((uint8) 0x00)						/* 未確定											*/
#define IGV_IN_AD_ST_NORMAL				((uint8) 0x01)						/* 確定												*/
#define IGV_IN_AD_ST_UNUSUAL			((uint8) 0x02)						/* 異常												*/

/* 電圧補正 */
#define OILPAD_CORRECT_COEFFICIENT		((uint32) 65536)					/* 精度向上補正係数									*/

/* サンプリング可否 */
#define OILPAD_SAMPSTS_NG				((uint8) 0x00)						/* サンプリング否									*/
#define OILPAD_SAMPSTS_OK				((uint8) 0x01)						/* サンプリング可									*/

/* IG電圧平均AD値 */
#define OILPAD_IGAD_INIT				((uint16) 0x0000)					/* IG電圧平均AD値:初期値							*/
#define OILPAD_IGAD_STS_UNKNOWN			((uint8)  0x00)						/* IG電圧AD値状態:未確定状態						*/
#define OILPAD_IGAD_STS_SIMPLE_AVRG		((uint8)  0x01)						/* IG電圧AD値状態:単純平均状態						*/
#define OILPAD_IGAD_STS_MOVE_AVRG		((uint8)  0x02)						/* IG電圧AD値状態:移動平均状態						*/
#define OILPAD_IGAD_AVRG_CNT			((uint8)  4)						/* IG電圧平均化回数									*/
#define OILPAD_IGAD_AVRG_INIT_CNT		((uint8)  0)						/* IG電圧蓄積カウンタ初期値							*/


#define OILP_UINT16_MAX					((uint16) 0xFFFF)					/* uint16型最大値									*/

/* AD値シフト */
#define OILPAD_AD_SHIFT_BIT				((uint16) 0x0002)					/* シフトビット数									*/
#define OILPAD_AD_MAX					((uint16) 0x03FF)					/* AD最大値											*/

/*============================================================================*
 * EXPORTED TYPEDEF DECLARATIONS                                              *
 *============================================================================*/

/*-----------------------------------------------------------------------------/
 *
 * Used Implementation Data Types
 *  uint8
 *  uint16
 *  uint8
 *  sint16
 *  uint32
 *  uint16
 *  pvU1: Record with 2 fields.
 *  pvU2: Record with 2 fields.
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
 * GLOBAL PARAMETERS		                                                  *
 *============================================================================*/


/*============================================================================*
 * STATIC PARAMETERS		                                                  *
 *============================================================================*/
#define swc_in_oilp_START_SEC_VAR_INIT_8
#include "swc_in_oilp_MemMap.h"

static uint8 igvAdSts;															/* IG電圧A/D値データ状態										*/
static uint8 oilpAdIgAdAvrgSts;													/* IG電圧平均AD値状態											*/
static uint8 oilpAdIgAdAccumuCnt;												/* IG電圧平均AD累積カウンタ										*/

#define swc_in_oilp_STOP_SEC_VAR_INIT_8
#include "swc_in_oilp_MemMap.h"


#define swc_in_oilp_START_SEC_VAR_INIT_16
#include "swc_in_oilp_MemMap.h"

static uint16 igvAd;															/* IG電圧A/D値													*/
static uint16 oilpAdPreIgAdAvrg;												/* 前回IG電圧平均AD値											*/

#define swc_in_oilp_STOP_SEC_VAR_INIT_16
#include "swc_in_oilp_MemMap.h"


#define swc_in_oilp_START_SEC_VAR_INIT_UNSPECIFIED
#include "swc_in_oilp_MemMap.h"

static uint16 oilpAdIgAdAvrgBuf[OILPAD_IGAD_AVRG_CNT];							/* IG電圧平均AD値バッファ										*/
static pvU1	  pvEngOnOff3s;														/* ＥＮＧ　ＯＮ／ＯＦＦ判定(フェール時間3s)の物理値				*/

#define swc_in_oilp_STOP_SEC_VAR_INIT_UNSPECIFIED
#include "swc_in_oilp_MemMap.h"


/*============================================================================*
 * STATIC FUNCTIONS PROTOTYPES                                                *
 *============================================================================*/
static void initPvOilp(void);													/* 初期化処理											*/
static uint16 makeNe1Signed(void);												/* エンジン回転数signed生成処理							*/
static void makeEngRevFail3s(uint16 *engRev3s, uint8 *engRevSts3s);				/* エンジン回転数(フェール時間3s)生成処理				*/
static void makeEngOnOff3s(void);												/* ＥＮＧ　ＯＮ／ＯＦＦ判定(フェール時間3s)生成処理		*/
static void makeIgv(void);														/* IG電圧入力情報生成処理								*/
static void makeOilpAd(void);													/* オイルプレッシャセンダ信号情報生成処理				*/
static uint8 jdgSampSts(void);													/* サンプリング開始可否判定処理							*/
static uint16 calcIgAdAvrgData(uint8 sampSts);									/* IG電圧平均AD値算出処理								*/
static uint16 correctOilpAd(uint16 igAdAvrg, uint16 oilpAd);					/* センダ電圧補正処理									*/


/*============================================================================*
 * EXPORTED FUNCTIONS PROTOTYPES                                              *
 *============================================================================*/
/* Declaration of runnable entities ------------------------------------------*/

/*-----------------------------------------------------------------------------/
 *
 * Runnable Entity: sym_rbl_in_oilp
 *  Can be invoked concurrently: TRUE
 *  Events:
 *  - Triggerred by ev_te_in_oilp (TIMING-EVENT) once every 0.01 seconds
 *
 *-----------------------------------------------------------------------------/
 *
 * Implicit Sender-Reciever Data-Read-Access API(s): 
 * - uint16 Rte_IRead_rbl_in_oilp_rp_srIf_common_igOnTime_val(void)
 * - sint16 Rte_IRead_rbl_in_oilp_rp_srIf_in_NE1_val(void)
 * - uint8 Rte_IRead_rbl_in_oilp_rp_srIf_in_NE1Sts_val(void)
 *
 * Implicit Sender-Reciever Data-Write-Access API(s): 
 * - void Rte_IWrite_rbl_in_oilp_pp_srIf_pv_PvEngOnOff3s_struct(const pvU1* struct)
 * - pvU1* Rte_IWriteRef_rbl_in_oilp_pp_srIf_pv_PvEngOnOff3s_struct(void)
 * - void Rte_IWrite_rbl_in_oilp_pp_srIf_pv_PvOilpAd_struct(const pvU2* struct)
 * - pvU2* Rte_IWriteRef_rbl_in_oilp_pp_srIf_pv_PvOilpAd_struct(void)
 *
 * Synchronous Client-Server API(s): 
 * - Std_ReturnType Rte_Call_rp_csIf_timer_startTimer_op(uint16 id, uint8 type, uint16 period)
 * - Std_ReturnType Rte_Call_rp_csIf_timer_sts_op(uint16 id, uint8* sts)
 * - Std_ReturnType Rte_Call_rp_csIf_ioHwAb_normalAdcStsGet_op(uint32 id, uint8* sts)
 * - Std_ReturnType Rte_Call_rp_csIf_ioHwAb_normalAdcValGet_op(uint32 id, uint16* val)
 *
 * Mode API(s): 
 * - uint8 Rte_Mode_rp_msIf_ig_ModeDeclGroup_ig(void)
 *
 *----------------------------------------------------------------------------*/

#define swc_in_oilp_START_SEC_CODE
#include "swc_in_oilp_MemMap.h"

/********************************************************************************/
/*	関数概要：オイルプレッシャゲージ(指針表示)(入力) タスク処理					*/
/*	処理時期：																	*/
/*																				*/
/*	引数	：無し																*/
/*	戻り値	：無し																*/
/*	注意事項：無し																*/
/********************************************************************************/
FUNC(void, swc_in_oilp_CODE) sym_rbl_in_oilp(void)
{
	makeEngOnOff3s();														/* ＥＮＧ　ＯＮ／ＯＦＦ判定(フェール時間3s)生成処理	*/
	makeIgv();																/* IG電圧入力情報生成処理							*/
	makeOilpAd();															/* オイルプレッシャセンダ信号情報生成処理			*/
}

#define swc_in_oilp_STOP_SEC_CODE
#include "swc_in_oilp_MemMap.h"


#define swc_in_oilp_START_SEC_CODE
#include "swc_in_oilp_MemMap.h"

/********************************************************************************/
/*	関数概要：オイルプレッシャゲージ(指針表示)(入力) 初期化処理（リセット）		*/
/*	処理時期：																	*/
/*																				*/
/*	引数	：無し																*/
/*	戻り値	：無し																*/
/*	注意事項：無し																*/
/********************************************************************************/
FUNC(void, swc_in_oilp_CODE) sym_rbl_in_oilp_initReset(void)
{
	pvU2 oilpAd;																/* OIL-P電圧A/D値物理値								*/

	initPvOilp();

	oilpAd.dt = PV_OILPAD_MIN;													/* 物理値をRTEへ出力								*/
	oilpAd.sts = PV_STS_UNKNOWN;

	Rte_IWrite_rbl_in_oilp_initReset_pp_srIf_pv_PvEngOnOff3s_struct(&pvEngOnOff3s);
	Rte_IWrite_rbl_in_oilp_initReset_pp_srIf_pv_PvOilpAd_struct(&oilpAd);
}

#define swc_in_oilp_STOP_SEC_CODE
#include "swc_in_oilp_MemMap.h"


#define swc_in_oilp_START_SEC_CODE
#include "swc_in_oilp_MemMap.h"

/********************************************************************************/
/*	関数概要：オイルプレッシャゲージ(指針表示)(入力) 初期化処理					*/
/*			  （ウェイクアップ）												*/
/*	処理時期：																	*/
/*																				*/
/*	引数	：無し																*/
/*	戻り値	：無し																*/
/*	注意事項：無し																*/
/********************************************************************************/
FUNC(void, swc_in_oilp_CODE) sym_rbl_in_oilp_initWakeup(void)
{
	pvU2 oilpAd;																/* OIL-P電圧A/D値物理値								*/

	initPvOilp();

	oilpAd.dt = PV_OILPAD_MIN;													/* 物理値をRTEへ出力								*/
	oilpAd.sts = PV_STS_UNKNOWN;
	Rte_IWrite_rbl_in_oilp_initWakeup_pp_srIf_pv_PvEngOnOff3s_struct(&pvEngOnOff3s);
	Rte_IWrite_rbl_in_oilp_initWakeup_pp_srIf_pv_PvOilpAd_struct(&oilpAd);
}

#define swc_in_oilp_STOP_SEC_CODE
#include "swc_in_oilp_MemMap.h"


#define swc_in_oilp_START_SEC_CODE
#include "swc_in_oilp_MemMap.h"

/********************************************************************************/
/*	関数概要：初期化処理														*/
/*	処理時期：																	*/
/*																				*/
/*	引数	：無し																*/
/*	戻り値	：無し																*/
/*	注意事項：無し																*/
/********************************************************************************/
static void initPvOilp(void)
{
	uint8 cnt;								/* ループカウンタ						*/

	pvEngOnOff3s.dt = PV_ENGONOFF3S_OFF;
	pvEngOnOff3s.sts = PV_STS_NOTRCV;

	igvAd = IGV_IN_IG_AD_MIN;
	igvAdSts = IGV_IN_AD_ST_INACTIVE;

	oilpAdIgAdAvrgSts = OILPAD_IGAD_STS_UNKNOWN;								/* 3	: IG電圧平均AD値状態にIG電圧AD値状態:未確定状態を設定			*/
	oilpAdIgAdAccumuCnt = OILPAD_IGAD_AVRG_INIT_CNT;							/* 4	: IG電圧平均AD累積カウンタにIG電圧蓄積カウンタ初期値を設定		*/
	oilpAdPreIgAdAvrg = OILPAD_IGAD_INIT;										/* 5	: 前回IG電圧平均AD値にIG電圧平均AD値:初期値を設定				*/

	for(cnt=0; cnt<OILPAD_IGAD_AVRG_CNT; cnt++){								/* 6	: IG電圧平均化回数分，以下の処理を実施							*/
		oilpAdIgAdAvrgBuf[cnt] = OILPAD_IGAD_INIT;								/* 		: IG電圧平均AD値バッファにIG電圧平均AD値:初期値を設定			*/
	}
}

#define swc_in_oilp_STOP_SEC_CODE
#include "swc_in_oilp_MemMap.h"


#define swc_in_oilp_START_SEC_CODE
#include "swc_in_oilp_MemMap.h"

/********************************************************************************/
/*	関数概要：エンジン回転数signed生成処理										*/
/*	処理時期：																	*/
/*																				*/
/*	引数	：無し																*/
/*	戻り値	：エンジン回転数(LSB:0.78125(rpm))									*/
/*	注意事項：無し																*/
/********************************************************************************/
static uint16 makeNe1Signed(void)
{
	uint16 ne1;										/*	エンジン回転数				*/
	sint16 inNe1Signed;								/*	signed型のエンジン回転数	*/

	inNe1Signed = Rte_IRead_rbl_in_oilp_rp_srIf_in_NE1_val();
													/*	1		：エンジン回転数signed信号取得処理を実施									*/
													/*			：signed型のエンジン回転数に「エンジン回転数signed信号」を設定				*/
													/*	1-1		：エンジン回転数の範囲判定													*/
	if(inNe1Signed < IN_NE1_MIN){					/*	1-1-1	：signed型のエンジン回転数がsigned型のエンジン回転数最小値より小さい場合	*/
		ne1 = NE1SIGNED_IN_DATA_MIN;				/*			：エンジン回転数に「エンジン回転数signedの最小値」を設定					*/

	}else{											/*	1-1-2	：上記以外の場合															*/
		ne1 = (uint16)inNe1Signed;					/*			：エンジン回転数にsigned型のエンジン回転数を設定							*/
	}
	return ne1;
}

#define swc_in_oilp_STOP_SEC_CODE
#include "swc_in_oilp_MemMap.h"


#define swc_in_oilp_START_SEC_CODE
#include "swc_in_oilp_MemMap.h"

/********************************************************************************/
/*	関数概要：エンジン回転数(フェール時間3s)生成処理							*/
/*	処理時期：																	*/
/*																				*/
/*	引数	：第一引数：エンジン回転数											*/
/*			：第ニ引数：エンジン回転数データ状態								*/
/*	戻り値	：無し																*/
/*	注意事項：無し																*/
/********************************************************************************/
static void makeEngRevFail3s(uint16 *engRev3s, uint8 *engRevSts3s)
{
	uint8 igSts;										/*	IG情報							*/
	uint8 ne1Sts;										/*	エンジン回転数データ状態取得値	*/

	igSts = Rte_Mode_rp_msIf_ig_ModeDeclGroup_ig();		/*	1				：IG情報を取得し、IG-ON/OFF判定							*/

	if(igSts == RTE_MODE_IG7V_ON){						/*	1-1				：IG-ON中の場合											*/

		ne1Sts = Rte_IRead_rbl_in_oilp_rp_srIf_in_NE1Sts_val();
														/*	1-1-1			：エンジン回転数途絶情報(3s)を取得し、途絶判定			*/

		if((ne1Sts & IN_MSGSTS_FAILINIT) == IN_MSGSTS_NONE){
														/*	1-1-1-1			：途絶未発生の場合										*/

			if((ne1Sts & IN_MSGSTS_NORX) == IN_MSGSTS_NONE){
														/*	1-1-1-1-1-1		：受信の場合											*/

				*engRev3s = makeNe1Signed();			/*	1-1-1-1-1-1-1	：エンジン回転数signed生成処理をコール					*/
														/*	1-1-1-1-1-1-2	：データの範囲チェック									*/
				if(*engRev3s > ENGREV3S_IN_MAX){		/*	1-1-1-1-1-1-2-1	：エンジン回転数がエンジン回転数の最大値より大きい場合	*/
					*engRev3s = ENGREV3S_IN_MAX;		/*					：エンジン回転数にエンジン回転数の最大値を設定			*/
				}else{									/*	1-1-1-1-1-1-2-2	：上記以外の場合										*/
					;									/*					：処理無し												*/
				}
				*engRevSts3s = ENGREV3S_IN_STS_NORMAL;	/*	1-1-1-1-1-1-3	：エンジン回転数データ状態に、「正常」を設定			*/
			}else{										/*	1-1-1-1-1-2		：受信以外の場合										*/

				*engRev3s = ENGREV3S_IN_MIN;			/*	1-1-1-1-1-2-1	：エンジン回転数に、「エンジン回転数の最小値」を設定	*/
				*engRevSts3s = ENGREV3S_IN_STS_UNFIX;	/*	1-1-1-1-1-2-2	：エンジン回転数データ状態に、「未確定」を設定			*/
			}
		}else{											/*	1-1-1-2			：途絶未発生以外の場合									*/
			*engRev3s = ENGREV3S_IN_MIN;				/*					：エンジン回転数に、「エンジン回転数の最小値」を設定	*/
			*engRevSts3s = ENGREV3S_IN_STS_FAIL;		/*					：エンジン回転数データ状態に、「フェール中」を設定		*/
		}
	}else{												/*	1-2				：IG-ON以外の場合										*/
		*engRev3s = ENGREV3S_IN_MIN;					/*					：エンジン回転数に、「エンジン回転数の最小値」を設定	*/
		*engRevSts3s = ENGREV3S_IN_STS_NORMAL;			/*					：エンジン回転数データ状態に、「正常」を設定			*/
	}
}

#define swc_in_oilp_STOP_SEC_CODE
#include "swc_in_oilp_MemMap.h"


#define swc_in_oilp_START_SEC_CODE
#include "swc_in_oilp_MemMap.h"

/********************************************************************************/
/*	関数概要：ＥＮＧ　ＯＮ／ＯＦＦ判定(フェール時間3s)生成処理					*/
/*	処理時期：																	*/
/*																				*/
/*	引数	：無し																*/
/*	戻り値	：無し																*/
/*	注意事項：無し																*/
/********************************************************************************/
static void makeEngOnOff3s(void)
{
	uint16 inputNe1;													/* エンジン回転数										*/
	uint8  igSts;														/* IG情報												*/
	uint8  ne1Sts;														/* エンジン回転数データ状態								*/
	uint8  usedInfo;													/* 信号有無情報											*/

	usedInfo = ENGONOFF3S_GET_CM_NE1();									/* 1,2				: エンジン回転数設定を取得								*/
	if( usedInfo == ENGONOFF3S_CM_NE1_USED ) {							/* 1				: エンジン回転数設定が使用 の場合						*/
		igSts = Rte_Mode_rp_msIf_ig_ModeDeclGroup_ig();					/* 1-1,1-2			: IG情報を取得											*/
		if( igSts == RTE_MODE_IG7V_ON ) { 								/* 1-1				: IG-ONの場合											*/
			makeEngRevFail3s(&inputNe1, &ne1Sts);						/*					: エンジン回転数データ状態(フェール時間3s)、			*/
																		/*					: エンジン回転数(フェール時間3s)を生成					*/
			if( ne1Sts == ENGREV3S_IN_STS_NORMAL ) {					/* 1-1-1			: 正常の場合											*/
				if( inputNe1 >= ENGONOFF3S_IN_NE1_ON_LIMIT ) {			/* 1-1-1-1			: エンジンＯＮ判定閾値以上の場合						*/
					pvEngOnOff3s.dt = PV_ENGONOFF3S_ON;					/*					: ＥＮＧ　ＯＮ／ＯＦＦ判定の物理値←「ON」				*/
				} else if( inputNe1 < ENGONOFF3S_IN_NE1_OFF_LIMIT ) {	/* 1-1-1-2			: エンジンＯＦＦ判定閾値より小さい場合					*/
					pvEngOnOff3s.dt = PV_ENGONOFF3S_OFF; 				/*					: ＥＮＧ　ＯＮ／ＯＦＦ判定の物理値←「OFF」				*/
				} else {												/* 1-1-1-3			: 上記以外の場合										*/
					;
				}
				pvEngOnOff3s.sts = PV_STS_NORMAL;						/* 1-1-1-4			: ＥＮＧ　ＯＮ／ＯＦＦ判定データ状態の物理値←「正常」	*/
			} else if( ne1Sts == ENGREV3S_IN_STS_FAIL ) {				/* 1-1-2			: フェール中の場合										*/
				pvEngOnOff3s.dt = PV_ENGONOFF3S_OFF; 					/*					: ＥＮＧ　ＯＮ／ＯＦＦ判定の物理値←「OFF」 			*/
				pvEngOnOff3s.sts = PV_STS_FAIL;							/*					: ＥＮＧ　ＯＮ／ＯＦＦ判定データ状態の物理値←「ﾌｪｰﾙ中」*/
			} else {													/* 1-1-3			: 上記以外の場合										*/
				pvEngOnOff3s.dt = PV_ENGONOFF3S_OFF;					/*					: ＥＮＧ　ＯＮ／ＯＦＦ判定の物理値←「OFF」 			*/
				pvEngOnOff3s.sts = PV_STS_NOTRCV;						/*					: ＥＮＧ　ＯＮ／ＯＦＦ判定データ状態の物理値←「未確定」*/
			}
		} else {														/* 1-2				: IG-ON以外の場合										*/
			pvEngOnOff3s.dt = PV_ENGONOFF3S_OFF;						/*					: ＥＮＧ　ＯＮ／ＯＦＦ判定の物理値←「OFF」 			*/
			pvEngOnOff3s.sts = PV_STS_NOTRCV;							/*					: ＥＮＧ　ＯＮ／ＯＦＦ判定データ状態の物理値←「未確定」*/
		}
	} else {															/* 2				: エンジン回転数設定が使用 以外の場合					*/
		pvEngOnOff3s.dt = PV_ENGONOFF3S_OFF;							/*					: ＥＮＧ　ＯＮ／ＯＦＦ判定の物理値←「OFF」 			*/
		pvEngOnOff3s.sts = PV_STS_NOTRCV;								/*					: ＥＮＧ　ＯＮ／ＯＦＦ判定データ状態の物理値←「未確定」*/
	}
																		/*					: 物理値をRTEへ出力										*/
	Rte_IWrite_rbl_in_oilp_pp_srIf_pv_PvEngOnOff3s_struct(&pvEngOnOff3s);
}

#define swc_in_oilp_STOP_SEC_CODE
#include "swc_in_oilp_MemMap.h"


#define swc_in_oilp_START_SEC_CODE
#include "swc_in_oilp_MemMap.h"

/********************************************************************************/
/*	関数概要：IG電圧入力情報生成処理											*/
/*	処理時期：																	*/
/*																				*/
/*	引数	：無し																*/
/*	戻り値	：無し																*/
/*	注意事項：無し																*/
/********************************************************************************/
static void makeIgv(void)
{
	uint16			 igvTimeSts;							/*	IG-ON後経過時間				*/
	uint8			 adSts;									/*	A/D変換状態					*/
	uint16			 igvAdInfor;							/*	IG電圧A/D値					*/
	Std_ReturnType	 csIfGetRet;							/*  csIf(get)戻り値				*/

	csIfGetRet = RTE_E_OK;
	igvTimeSts = Rte_IRead_rbl_in_oilp_rp_srIf_common_igOnTime_val();
															/*	1		：IG-ON後経過時間取得処理を実施し，IG-ON後経過時間を取得する。			*/
	if(igvTimeSts >= IGV_STABILITY_PERIOD){					/*	2		：IG-ON後経過時間(戻り値)≧IG電源安定化待ち時間の場合					*/
		csIfGetRet = Rte_Call_rp_csIf_ioHwAb_normalAdcStsGet_op(IN_NORMALADC_CH_3, &adSts);
															/*			：A/D変換状態取得処理を実施し，A/D変換状態を取得する。					*/
		if(csIfGetRet == RTE_E_OK){
			if(adSts == AD_STS_NORMAL){						/*	2-1		：A/D変換状態(戻り値)が【確定】の場合									*/
				csIfGetRet = Rte_Call_rp_csIf_ioHwAb_normalAdcValGet_op(IN_NORMALADC_CH_3, &igvAdInfor);
															/*			：IG電圧A/D値取得処理を実施し，IG電圧A/D値を取得する。					*/
				if(csIfGetRet == RTE_E_OK){
					igvAdInfor = igvAdInfor >> OILPAD_AD_SHIFT_BIT;
					if(OILPAD_AD_MAX < igvAdInfor){			/*	2-1-1	：IG電圧A/D値が範囲外の場合												*/
						igvAd = IGV_IN_IG_AD_MIN;			/*			：IG電圧A/D値に「IG電圧A/D値：最小値」を設定する。						*/
						igvAdSts = IGV_IN_AD_ST_UNUSUAL;	/*			：IG電圧A/D値データ状態に「異常」を設定する。							*/
					}else{									/*	2-1-2	：IG電圧A/D値(戻り値)が上記以外の場合									*/
						igvAd = igvAdInfor;					/*			：IG電圧A/D値に「IG電圧A/D値(戻り値)」を設定する。						*/
						igvAdSts = IGV_IN_AD_ST_NORMAL;		/*			：IG電圧A/D値データ状態に「確定」を設定する。							*/
					}
				} else {
					igvAd = IGV_IN_IG_AD_MIN;
					igvAdSts = IGV_IN_AD_ST_INACTIVE;
				}
			}else if(adSts == AD_STS_ABNORMAL){				/*	2-2		：A/D変換状態(戻り値)が【異常】の場合									*/
				igvAd = IGV_IN_IG_AD_MIN;					/*			：IG電圧A/D値に「IG電圧A/D値：最小値」を設定する。						*/
				igvAdSts = IGV_IN_AD_ST_UNUSUAL;			/*			：IG電圧A/D値データ状態に「異常」を設定する。							*/
			}else if(adSts == AD_STS_UNKNOWN){				/*	2-3		：A/D変換状態(戻り値)が【未確定】の場合									*/
				igvAd = IGV_IN_IG_AD_MIN;					/*			：IG電圧A/D値に「IG電圧A/D値：最小値」を設定する。						*/
				igvAdSts = IGV_IN_AD_ST_INACTIVE;			/*			：IG電圧A/D値データ状態に「未確定」を設定する。							*/
			}else{											/*	2-4		：A/D変換状態(戻り値)が上記以外の場合									*/
				igvAd = IGV_IN_IG_AD_MIN;					/*			：IG電圧A/D値に「IG電圧A/D値：最小値」を設定する。						*/
				igvAdSts = IGV_IN_AD_ST_INACTIVE;			/*			：IG電圧A/D値データ状態に「未確定」を設定する。							*/
			}
		} else {
			igvAd = IGV_IN_IG_AD_MIN;
			igvAdSts = IGV_IN_AD_ST_INACTIVE;
		}
	}else{													/*	3		：IG-ON後経過時間(戻り値)が上記以外の場合								*/
		igvAd = IGV_IN_IG_AD_MIN;							/*			：IG電圧A/D値に「IG電圧A/D値：最小値」を設定する。						*/
		igvAdSts = IGV_IN_AD_ST_INACTIVE;					/*			：IG電圧A/D値データ状態に「未確定」を設定する。							*/
	}
}

#define swc_in_oilp_STOP_SEC_CODE
#include "swc_in_oilp_MemMap.h"


#define swc_in_oilp_START_SEC_CODE
#include "swc_in_oilp_MemMap.h"

/********************************************************************************/
/*	関数概要：オイルプレッシャセンダ信号情報生成処理							*/
/*	処理時期：																	*/
/*																				*/
/*	引数	：無し																*/
/*	戻り値	：無し																*/
/*	注意事項：無し																*/
/********************************************************************************/
static void makeOilpAd(void)
{
	uint8			 sampSts;							/* サンプリング開始可否判定				*/
	uint16			 igAdAvrg;							/* IG電圧平均AD値						*/
	uint8			 adSts;								/* A/D変換状態							*/
	uint16			 correctResult;						/* 補正オイルプレッシャセンダ電圧AD値	*/
	pvU2			 oilpAd;							/* OIL-P電圧A/D値出力値					*/
	uint16			 inputH2k;							/* オイルプレッシャセンダ電圧			*/
	Std_ReturnType	 csIfGetRet;						/*  csIf(get)戻り値						*/

	csIfGetRet = RTE_E_OK;
	sampSts = jdgSampSts();															/* 1		: サンプリング開始可否判定を取得							*/
	igAdAvrg = calcIgAdAvrgData(sampSts);											/* 2		: IG電圧平均AD値を取得										*/

	if(sampSts == OILPAD_SAMPSTS_OK){												/* 3		: サンプリング開始可否判定が【サンプリング可】の場合		*/
		csIfGetRet = Rte_Call_rp_csIf_ioHwAb_normalAdcStsGet_op(IN_NORMALADC_CH_0, &adSts);	/* 		: オイルプレッシャセンダ電圧のA/D変換状態を取得			*/
		if(csIfGetRet == RTE_E_OK){
			switch(adSts){
			case AD_STS_NORMAL:														/* 3-1		: A/D変換状態が【確定】の場合								*/
				csIfGetRet = Rte_Call_rp_csIf_ioHwAb_normalAdcValGet_op(IN_NORMALADC_CH_0, &inputH2k);
				inputH2k = inputH2k >> OILPAD_AD_SHIFT_BIT;
				if(csIfGetRet == RTE_E_OK){
					correctResult = correctOilpAd(igAdAvrg, inputH2k);				/* 			: 補正オイルプレッシャセンダ電圧AD値を取得					*/
					if(correctResult >= PV_OILPAD_MAX){								/* 3-1-1	: 補正オイルプレッシャセンダ電圧AD値が						*/
																					/* 								【OIL-P電圧A/D値:最大値】以上の場合		*/
						oilpAd.dt = PV_OILPAD_MAX;									/* 			: OIL-P電圧A/D値にOIL-P電圧A/D値:最大値を設定				*/
					}else{															/* 3-1-2	: 補正オイルプレッシャセンダ電圧AD値が上記以外の場合		*/
						oilpAd.dt = correctResult;									/* 			: OIL-P電圧A/D値に補正オイルプレッシャセンダ電圧AD値を設定	*/
					}
					oilpAd.sts = PV_STS_NORMAL;										/* 3-1-3	: OIL-P電圧A/D値データ状態に確定を設定						*/
				} else {
					oilpAd.dt = PV_OILPAD_MIN;
					oilpAd.sts = PV_STS_UNKNOWN;
				}
				break;
			case AD_STS_ABNORMAL:													/* 3-2		: A/D変換状態が【異常】の場合								*/
				oilpAd.dt = PV_OILPAD_MIN;											/* 			: OIL-P電圧A/D値にOIL-P電圧A/D値:最小値を設定				*/
				oilpAd.sts = PV_STS_ERR;											/* 			: OIL-P電圧A/D値データ状態に異常を設定						*/
				break;
			case AD_STS_UNKNOWN:													/* 3-3		: A/D変換状態が【未確定】の場合								*/
				/*	Fall Through	*/
			default:																/* 3-4		: A/D変換状態が上記以外の場合								*/
				oilpAd.dt = PV_OILPAD_MIN;											/* 3-3,3-4	: OIL-P電圧A/D値にOIL-P電圧A/D値:最小値を設定				*/
				oilpAd.sts = PV_STS_UNKNOWN;										/* 			: OIL-P電圧A/D値データ状態に未確定を設定					*/
				break;
			}
		} else {
			oilpAd.dt = PV_OILPAD_MIN;
			oilpAd.sts = PV_STS_UNKNOWN;
		}

	}else{																			/* 4		: サンプリング開始可否判定が上記以外の場合					*/
		oilpAd.dt = PV_OILPAD_MIN;													/* 			: OIL-P電圧A/D値にOIL-P電圧A/D値:最小値を設定				*/
		oilpAd.sts = PV_STS_UNKNOWN;												/* 			: OIL-P電圧A/D値データ状態に未確定を設定					*/
	}

	Rte_IWrite_rbl_in_oilp_pp_srIf_pv_PvOilpAd_struct(&oilpAd);						/*			: 物理値をRTEへ出力											*/
}

#define swc_in_oilp_STOP_SEC_CODE
#include "swc_in_oilp_MemMap.h"


#define swc_in_oilp_START_SEC_CODE
#include "swc_in_oilp_MemMap.h"

/********************************************************************************/
/*	関数概要：サンプリング開始可否判定処理										*/
/*	処理時期：																	*/
/*																				*/
/*	引数	：無し																*/
/*	戻り値	：サンプリング開始可否判定											*/
/*	注意事項：無し																*/
/********************************************************************************/
static uint8 jdgSampSts(void)
{
	uint8 sampSts;							/* サンプリング開始可否判定				*/

	sampSts = OILPAD_SAMPSTS_NG;												/* 1	: サンプリング開始可否判定にサンプリング否を設定				*/

	if(igvAdSts != IGV_IN_AD_ST_INACTIVE){										/* 3	: IG電圧A/D値データ状態が【未確定】以外の場合					*/
		sampSts = OILPAD_SAMPSTS_OK;											/* 		: サンプリング開始可否判定にサンプリング可を設定				*/
	}

	return sampSts;																/* 4	: サンプリング開始可否判定を返却								*/
}

#define swc_in_oilp_STOP_SEC_CODE
#include "swc_in_oilp_MemMap.h"


#define swc_in_oilp_START_SEC_CODE
#include "swc_in_oilp_MemMap.h"

/********************************************************************************/
/*	関数概要：IG電圧平均AD値算出処理											*/
/*	処理時期：																	*/
/*																				*/
/*	引数	：サンプリング開始可否判定											*/
/*	戻り値	：IG電圧平均AD値													*/
/*	注意事項：無し																*/
/********************************************************************************/
static uint16 calcIgAdAvrgData(uint8 sampSts)
{
	uint16		   igAdAvrg;						/* IG電圧平均AD値						*/
	uint16		   igAd;							/* IG電圧A/D値							*/
	uint8		   tmrSts;							/* タイマ状態							*/
	uint8		   cnt;								/* ループカウンタ						*/
	uint32		   work=0;							/* 加算結果格納用メモリ					*/
	Std_ReturnType ret;								/* csIf実施結果							*/

	if(sampSts == OILPAD_SAMPSTS_NG){											/* 1,3,7			: サンプリング開始可否判定が【サンプリング否】の場合		*/
		oilpAdIgAdAvrgSts = OILPAD_IGAD_STS_UNKNOWN;							/* 1				: IG電圧平均AD値状態にIG電圧AD値状態:未確定状態を設定		*/

		for(cnt=0; cnt<OILPAD_IGAD_AVRG_CNT; cnt++){							/* 3				: IG電圧平均AD値バッファの初期化							*/
			oilpAdIgAdAvrgBuf[cnt] = OILPAD_IGAD_INIT;
		}
		oilpAdIgAdAccumuCnt = OILPAD_IGAD_AVRG_INIT_CNT;						/* 					: IG電圧平均AD累積カウンタの初期化							*/
		igAdAvrg = OILPAD_IGAD_INIT;											/* 					: IG電圧平均AD値にIG電圧平均AD値:初期値を設定				*/

	}else{																		/* 2,4,5,6			: サンプリング開始可否判定が【サンプリング可】の場合		*/
		igAd = igvAd + OILPAD_IG_OFFSET_AD;										/* 4,5				: IG電圧A/D値を取得											*/

		if(oilpAdIgAdAvrgSts == OILPAD_IGAD_STS_UNKNOWN){						/* 2-1,4-1,6-1		: IG電圧平均AD値状態が【IG電圧AD値状態:未確定状態】の場合	*/
			oilpAdIgAdAvrgSts = OILPAD_IGAD_STS_SIMPLE_AVRG;					/* 2-1				: IG電圧平均AD値状態にIG電圧AD値状態:単純平均状態を設定		*/

			for(cnt=0; cnt<OILPAD_IGAD_AVRG_CNT; cnt++){						/* 4-1-1 			: IG電圧平均化回数分，以下の処理を実施						*/
				oilpAdIgAdAvrgBuf[cnt] = igAd;									/* 					: IG電圧平均AD値バッファにIG電圧A/D値を設定					*/
			}
			oilpAdIgAdAccumuCnt = OILPAD_IGAD_AVRG_INIT_CNT;					/* 4-1-2			: IG電圧平均AD累積カウンタにIG電圧蓄積カウンタ初期値を設定	*/
			igAdAvrg = igAd;													/* 4-1-3			: IG電圧平均AD値にIG電圧A/D値を設定							*/

																				/* 6-1				: IG電圧平均化サンプリング時間用開始要求処理を実施			*/
			ret = Rte_Call_rp_csIf_timer_startTimer_op(TIMID_IN_OILP_IGAD, TM_TIMTYP_CYCLIC, OILPAD_TMR_PERIOD);
			if(ret != RTE_E_OK){
				;
			}else{
				;
			}
		}else{																	/* 2-2,5-1,6-2		: IG電圧平均AD値状態が【IG電圧AD値状態:単純平均状態】または	*/
																				/* 					: IG電圧平均AD値状態が【IG電圧AD値状態:移動平均状態】の場合	*/
			ret = Rte_Call_rp_csIf_timer_sts_op(TIMID_IN_OILP_IGAD, &tmrSts);	/* 6-2				: IG電圧平均化サンプリング時間用タイマ状態を取得			*/
			if(ret != RTE_E_OK){
				igAdAvrg = oilpAdPreIgAdAvrg;									/* 					: 前回IG電圧平均AD値は保持									*/
			}else{
				if(tmrSts == TM_TIMSTS_TIMEOUT){								/* 2-2-1,5-1-1		: タイマ状態が【タイムアウト済】の場合						*/
					oilpAdIgAdAvrgSts = OILPAD_IGAD_STS_MOVE_AVRG;				/* 2-2-1			: IG電圧平均AD値状態にIG電圧AD値状態:移動平均状態を設定		*/

					if(oilpAdIgAdAccumuCnt >= OILPAD_IGAD_AVRG_CNT){			/* 5-1-1-2			: IG電圧平均累積カウンタが【IG電圧平均化回数】以上の場合	*/
						oilpAdIgAdAccumuCnt = OILPAD_IGAD_AVRG_INIT_CNT;		/* 					: IG電圧平均AD累積カウンタにIG電圧蓄積カウンタ初期値を設定	*/
					}else{														/* 5-1-1-1			: IG電圧平均累積カウンタが【IG電圧平均化回数】未満の場合	*/
						;
					}
					oilpAdIgAdAvrgBuf[oilpAdIgAdAccumuCnt] = igAd;				/* 5-1-1-1,5-1-1-2	: IG電圧平均AD値バッファ[IG電圧平均累積カウンタ]に			*/
																				/* 					: 										IG電圧A/D値を設定	*/
					oilpAdIgAdAccumuCnt++;										/* 5-1-1-1,5-1-1-2	: IG電圧平均累積カウンタをインクリメント					*/

					for(cnt=0; cnt<OILPAD_IGAD_AVRG_CNT; cnt++){				/* 5-1-1-3			: IG電圧平均AD値バッファ[IG電圧平均累積カウンタ]を			*/
						work += oilpAdIgAdAvrgBuf[cnt];							/* 					: 							IG電圧平均化回数分，加算		*/
					}
					igAdAvrg = (uint16)(work / (uint32)OILPAD_IGAD_AVRG_CNT);	/* 5-1-1-4			: IG電圧平均AD値に除算結果を設定							*/

				}else{															/* 2-2-2,5-1-2		: タイマ状態が上記以外の場合								*/
																				/* 2-2-2			: IG電圧平均AD値状態は保持									*/
					igAdAvrg = oilpAdPreIgAdAvrg;								/* 5-1-2			: 前回IG電圧平均AD値は保持									*/
				}
			}
		}
	}

	oilpAdPreIgAdAvrg = igAdAvrg;												/* 7,8				: 前回IG電圧平均AD値にIG電圧平均AD値を設定					*/

	return igAdAvrg;															/* 9				: IG電圧平均AD値を返却										*/
}

#define swc_in_oilp_STOP_SEC_CODE
#include "swc_in_oilp_MemMap.h"


#define swc_in_oilp_START_SEC_CODE
#include "swc_in_oilp_MemMap.h"

/********************************************************************************/
/*	関数概要：センダ電圧補正処理												*/
/*	処理時期：																	*/
/*																				*/
/*	引数	：第一引数：IG電圧平均AD値											*/
/*			：第二引数：オイルプレッシャセンダ電圧								*/
/*	戻り値	：補正オイルプレッシャセンダ電圧AD値								*/
/*	注意事項：無し																*/
/********************************************************************************/
static uint16 correctOilpAd(uint16 igAdAvrg, uint16 oilpAd)
{
	uint16			 correctResult;					/* 補正オイルプレッシャセンダ電圧AD値	*/
	uint32			 temp;							/* 除数計算用格納メモリ					*/
	uint32			 stcDivisor;					/* 除数格納メモリ						*/
	uint32			 work;							/* 補正結果一時格納メモリ				*/

	if(igAdAvrg >= OILPAD_BATV_ADJ){											/* 2	: IG電圧平均AD値が【電圧補正閾値】以上の場合					*/
		temp = (uint32)(OILPAD_COEFFICIENT_A1 * igAdAvrg);						/* 		: オイルプレッシャセンダ電圧，精度向上補正係数，				*/
		if(OILPAD_COEFFICIENT_B1 > temp){										/* 		: 電圧補正係数A1，電圧補正係数B1，IG電圧平均AD値より			*/
			stcDivisor = OILPAD_COEFFICIENT_B1 - temp;							/* 		: 補正オイルプレッシャセンダ電圧AD値を算出						*/
		}else{
			stcDivisor = 0;
		}
	}else{																		/* 3	: IG電圧平均AD値が上記以外の場合								*/
		temp = (uint32)(OILPAD_COEFFICIENT_A2 * igAdAvrg);						/* 		: オイルプレッシャセンダ電圧，精度向上補正係数，				*/
		if(OILPAD_COEFFICIENT_B2 > temp){										/* 		: 電圧補正係数A2，電圧補正係数B2，IG電圧平均AD値より			*/
			stcDivisor = OILPAD_COEFFICIENT_B2 - temp;							/* 		: 補正オイルプレッシャセンダ電圧AD値を算出						*/
		}else{
			stcDivisor = 0;
		}
	}

	if(stcDivisor > 0){															/* 2，3	: オイルプレッシャセンダ電圧，精度向上補正係数，				*/
		work = (oilpAd * OILPAD_CORRECT_COEFFICIENT) / stcDivisor;				/* 		: 電圧補正係数A1/A2，電圧補正係数B1/B2，IG電圧平均AD値より		*/
		if(work > (uint32)OILP_UINT16_MAX){										/* 		: 補正オイルプレッシャセンダ電圧AD値を算出						*/
			correctResult = OILP_UINT16_MAX;
		}else{
			correctResult = (uint16)work;
		}
	}else{
		correctResult = OILP_UINT16_MAX;
	}

	return correctResult;														/* 4	: 補正オイルプレッシャセンダ電圧AD値を返却						*/
}

#define swc_in_oilp_STOP_SEC_CODE
#include "swc_in_oilp_MemMap.h"
