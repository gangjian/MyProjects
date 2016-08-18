/*	$RCSfile: Rte_swc_in_oilp_map.h $															*/
/*	$Date: 2015/11/26 20:56:22JST $																*/
/*	$Revision: 1.2 $																			*/
/*	 EXPLANATION: オイルプレッシャゲージ(指針表示)(入力) コンフィギュレーションヘッダファイル	*/

#ifndef RTE_SWC_IN_OILP_MAP_H
#define RTE_SWC_IN_OILP_MAP_H

/*------------------------------------------------------------------------------*/
/*	インクルードファイル														*/
/*------------------------------------------------------------------------------*/


/*------------------------------------------------------------------------------*/
/*	インクルードファイル (マイコン名のみ変更可)									*/
/*------------------------------------------------------------------------------*/

/*------------------------------------------------------------------------------*/
/*	マクロ定義（変更禁止）														*/
/*------------------------------------------------------------------------------*/

/*------------------------------------------------------------------------------*/
/*	コンフィギュレーション設定													*/
/*------------------------------------------------------------------------------*/
/* IG電源安定時間設定 */
#define IGV_STABILITY_PERIOD			((uint16)36)							/* IG電源安定時間(400ms)						:uint16 */

/* 電圧補正 */
#define OILPAD_BATV_ADJ					((uint16)650)							/* 電圧補正閾値									:uint16 */
#define OILPAD_COEFFICIENT_A1			((uint16)56)							/* 電圧補正係数A1								:uint16 */
#define OILPAD_COEFFICIENT_B1			((uint32)101895)						/* 電圧補正係数B1								:uint32 */
#define OILPAD_COEFFICIENT_A2			((uint16)100)							/* 電圧補正係数A2								:uint16 */
#define OILPAD_COEFFICIENT_B2			((uint32)130271)						/* 電圧補正係数B2								:uint32 */

/* IG電圧A/D値 */
#define OILPAD_IG_OFFSET_AD				((uint16)0)								/* ダイオードによる電圧降下分のA/D値			:uint16 */

/* タイマ開始要求 */
#define OILPAD_TMR_PERIOD				((uint16)80)							/* IG電圧平均化サンプリング時間					:uint16 */

/* エンジン回転数(フェール時間3s) */
#define ENGONOFF3S_IN_NE1_ON_LIMIT		((uint16)512)							/* エンジンＯＮ判定閾値							:uint16 */
#define ENGONOFF3S_IN_NE1_OFF_LIMIT		((uint16)256)							/* エンジンＯＦＦ判定閾値						:uint16 */

/* エンジン回転数設定 */
#define ENGONOFF3S_CM_NE1_UNUSED		((uint8)0x00)							/* 非適用										:uint8  */
#define ENGONOFF3S_CM_NE1_USED			((uint8)0x01)							/* 適用 										:uint8  */
#define ENGONOFF3S_GET_CM_NE1()			(ENGONOFF3S_CM_NE1_USED)				/* エンジン回転数設定取得処理					:uint8  */

#endif /* RTE_SWC_IN_OILP_MAP_H */
