/*	$RCSfile: bsw_common_map.h $									*/
/*	$Date: 2015/10/30 18:52:48JST $									*/
/*	$Revision: 1.8 $												*/
/*	 EXPLANATION: BSW共通ヘッダコンフィギュレーションファイル		*/

#ifndef BSW_COMMON_MAP_INC
#define BSW_COMMON_MAP_INC

/*------------------------------------------------------------------------------*/
/*	インクルードファイル														*/
/*------------------------------------------------------------------------------*/

/*------------------------------------------------------------------------------*/
/*	定数定義																	*/
/*------------------------------------------------------------------------------*/

/*------------------------------------------------------------------------------*/
/*	マクロ定義																	*/
/*------------------------------------------------------------------------------*/


/*------------------------------------------------------------------------------*/
/*	ROM化しない未使用機能設定													*/
/*------------------------------------------------------------------------------*/
/* 未使用入力信号 */
/* ↓ユーザー編集可能↓ */
#define	APP_UNUSED_IN_SIG_SI						/*	車速入力パルス（直線）								*/
#define	APP_UNUSED_IN_SIG_BCMASKTRCOFF				/*	TRC OFF バルブチェックマスク						*/
#define	APP_UNUSED_IN_SIG_BCPKB						/*	PKBスイッチ信号(B_CPKB)								*/
#define	APP_UNUSED_IN_SIG_PKB						/*	PKBスイッチ直線信号									*/
/* ↑ユーザー編集可能↑ */


/* 未使用出力信号 */
/* ↓ユーザー編集可能↓ */
#define	APP_UNUSED_OUT_SKEYDCON2_BZ					/*	スマートキー断続その２ブザー						*/
#define	APP_UNUSED_OUT_BKL_BZ						/*	シートベルトブザー									*/
#define	APP_UNUSED_OUT_EXIT_BZ						/*	非常ドアブザー										*/
#define	APP_UNUSED_PUB_DT_BZKIND					/*	ブザー種別設定										*/
#define	APP_UNUSED_FUNC_MULTI_MSG_EPS				/*	EPSメッセージ										*/
#define	APP_UNUSED_MLT_ENGWNG						/*	チェックエンジンウォーニング メッセージ				*/
#define	APP_UNUSED_MSG_4WDIND						/*	4WDテルテール メッセージ							*/
#define	APP_UNUSED_MSG_CNT_BRAKE					/*	ブレーキウォーニング(テルテール) メッセージカウンタ	*/

#define	APP_UNUSED_OUT_SIG_RW_TPMS_CAN_TIRW			/*	リモートウォーニング(タイヤ空気圧システム異常)		*/
#define	APP_UNUSED_OUT_SIG_RW_TPMS_CAN_TILW			/*	リモートウォーニング(タイヤ空気圧低下)				*/
#define	APP_UNUSED_OUT_SIG_RW_EPS					/*	EPSリモートウォーニング								*/
#define	APP_UNUSED_OUT_SIG_RW_SMBW					/*	オートマティックハイビーム リモートウォーニング		*/
#define	APP_UNUSED_OUT_SIG_RW_BSM					/*	BSMリモートウォーニング								*/
#define	APP_UNUSED_OUT_SIG_RW_CSR					/*	クリアランスソソナー表示 断線表示ウォーニング機能	*/
#define	APP_UNUSED_OUT_SIG_RW_TEMP					/*	水温リモートウォーニング							*/
#define	APP_UNUSED_OUT_SIG_RW_ALL					/*	通信出力リモートウォーニング						*/
#define	APP_UNUSED_OUT_SIG_RW_WASHERWNG				/*	WASHER (テルテール) リモートウォーニング			*/
#define	APP_UNUSED_OUT_SIG_RW_ABG					/*	ABGリモートウォーニング								*/
#define	APP_UNUSED_OUT_SIG_RW_ABS					/*	ABSリモートウォーニング								*/
#define	APP_UNUSED_OUT_SIG_RW_PCS					/*	PCSリモートウォーニング								*/
#define	APP_UNUSED_OUT_SIG_RW_SLIP					/*	SLIPリモートウォーニング							*/
#define	APP_UNUSED_OUT_SIG_RW_ENGWNG				/*	チェックエンジンウォーニング リモートウォーニング	*/
#define	APP_UNUSED_OUT_SIG_RW_4WDIND				/*	4WDテルテール リモートウォーニング					*/
#define	APP_UNUSED_OUT_SIG_RW_BRAKE					/*	ブレーキウォーニング(テルテール) リモートウォーニング	*/
/* ↑ユーザー編集可能↑ */



#endif	/* BSW_COMMON_MAP_INC */
