/*	$RCSfile: bsw_in_csmon.h $										*/
/*	$Date: 2015/10/27 14:53:49JST $									*/
/*	$Revision: 1.2 $												*/
/*	 EXPLANATION: BSW CSモニタ 公開ヘッダファイル					*/

#ifndef BSW_IN_CSMON_INC
#define BSW_IN_CSMON_INC

#ifndef BSW_IN_CSMON_DEF
#define BSW_IN_CSMON_EXT extern
#define BSW_IN_CSMON_EXT_C extern const
#else
#define BSW_IN_CSMON_EXT
#define BSW_IN_CSMON_EXT_C
#endif

/*------------------------------------------------------------------------------*/
/*	インクルードファイル														*/
/*------------------------------------------------------------------------------*/

/*------------------------------------------------------------------------------*/
/*	公開定義																	*/
/*------------------------------------------------------------------------------*/
/*----------------------------------------------------------*/
/* 【ネットワークステータス取得処理】						*/
/*----------------------------------------------------------*/
/* ネットワークステータス値 */
#define CSMON_NTWSTS_VALID				((uint16) 0x0000)		/* 以下のネットワークステータス情報が有効な値である状態を示す。		*/
#define CSMON_NTWSTS_INVALID			((uint16) 0x8000)		/* 以下のネットワークステータス情報が有効な値でない状態を示す。		*/
#define CSMON_NTWSTS_BUSSLEEPIND		((uint16) 0x0080)		/* 自ノードが送信するリングメッセージのSleep.ind状態を示す			*/
#define CSMON_NTWSTS_TXRINGDATAALLOWED 	((uint16) 0x0040)		/* リングメッセージ領域へのアクセス禁止状態を示す					*/
#define CSMON_NTWSTS_WAITBUSSLEEP		((uint16) 0x0020)		/* NMTwbsNormal又はNMTwbsLimphome状態を示す							*/
#define CSMON_NTWSTS_BUSSLEEP			((uint16) 0x0010)		/* NM BusSleep状態を示す											*/
#define CSMON_NTWSTS_LIMPHOME			((uint16) 0x0008)		/* NM Limphome状態を示す											*/
#define CSMON_NTWSTS_NMACTIVE			((uint16) 0x0004)		/* NM Active状態を示す												*/
#define CSMON_NTWSTS_BUSERROR			((uint16) 0x0002)		/* バスオフ状態を示す												*/
#define CSMON_NTWSTS_RINGSTABLE			((uint16) 0x0001)		/* ネットワーク状態が安定している（リングメッセージが一巡しても、	*/
																/* 論理リング構成に変更がない）ことを示す 							*/
/* ネットワークステータス取得処理 */
/*#define CSMON_GET_NM_STATUS() 		(CsMonNetworkStatus)*/	/* ネットワークステータスを取得します。								*/

/*----------------------------------------------------------*/
/* 【メッセージステータス】									*/
/*----------------------------------------------------------*/
/* メッセージステータス値 */
#define CSMON_MSGSTS_NONE				((uint8) 0x00)			/* 未発生状態														*/
#define CSMON_MSGSTS_NG					((uint8) 0x80)			/* 異常状態															*/
#define CSMON_MSGSTS_FAIL				((uint8) 0x20)			/* フェイル発生初期化済												*/
#define CSMON_MSGSTS_TXSTOP				((uint8) 0x10)			/* 送信一時停止状態													*/
#define CSMON_MSGSTS_TIMEOUT			((uint8) 0x02) 			/* タイムアウト発生													*/
#define CSMON_MSGSTS_NORX 				((uint8) 0x01)			/* 一度もメッセージを受信していない									*/

/*----------------------------------------------------------*/
/* 【メッセージステータス取得処理】							*/
/*----------------------------------------------------------*/
/*#define CSMON_GET_MSG_IDX_STATUS(sigName)	(CsMonMsgStatus[sigName])*/
															/* 「メッセージインデックス用信号名」により指定されたメッセージの	*/
															/* メッセージステータスを返します。									*/

/*----------------------------------------------------------*/
/* 【メッセージステータス抽出処理】							*/
/*----------------------------------------------------------*/
#define CSMON_GET_MSGSTS_BIT(msgSts,mask)		((uint8)((msgSts)&(mask)))
															/* 指定された「メッセージステータス」を「マスク用メッセージ			*/
															/* ステータス」にて論理ANDした値を返します。						*/

/*----------------------------------------------------------*/
/* 【メッセージ判定処理】									*/
/*----------------------------------------------------------*/
/*#define CSMON_TST_MSG_ST(sigName,mask)		CSMON_GET_MSGSTS_BIT(CSMON_GET_MSG_IDX_STATUS(sigName),mask)*/
															/* 「メッセージインデックス用信号名」により指定されたメッセージ		*/
															/* ステータスと「マスク用メッセージステータス」にて論理ANDした値を	*/
															/* 返します。														*/

/*----------------------------------------------------------*/
/* 【カスタマイズメッセージ判定処理】						*/
/*----------------------------------------------------------*/
/*#define CSMON_TST_CUST_MSG_ST(custSigName,mask)	CSMON_GET_MSGSTS_BIT(GetAppCsMonInCustMsgStatus(custSigName),mask)*/
															/* 「カスタマイズメッセージインデックス用信号名」により指定された	*/
															/* メッセージステータスと「マスク用メッセージステータス」にて論理	*/
															/* ANDした値を返します。											*/

/*----------------------------------------------------------*/
/* 【メッセージ受信カウンタ】								*/
/*----------------------------------------------------------*/
/* メッセージ受信カウンター値 */
#define CSMON_MSG_IDX_COUNTER_MIN			((uint8) 0)			/* メッセージ受信カウンタ：最小値 */
#define CSMON_MSG_IDX_COUNTER_MAX			((uint8) 0xFF)		/* メッセージ受信カウンタ：最大値 */

/* メッセージ受信カウンタ取得処理 */
/*#define CSMON_GET_MSG_IDX_COUNTER(sigName)	(CsMonMsgRcvCnt[sigName])*/
															/* 「メッセージインデックス用信号名」により指定されたメッセージの	*/
															/* メッセージ受信カウンタを返します。								*/

/*------------------------------------------------------------------------------*/
/*	公開変数																	*/
/*------------------------------------------------------------------------------*/
/*APP_IN_CSMON_EXT_C uint16	CsMonNetworkStatus;								*//* ネットワークステータス								*/
/*APP_IN_CSMON_EXT_C uint8	CsMonMsgStatus[CSMON_MSG_IDX_NUM];				*//* メッセージステータス									*/
/*APP_IN_CSMON_EXT_C uint8	CsMonMsgRcvCnt[CSMON_MSG_IDX_NUM];				*//* メッセージ受信カウンタ								*/

/*------------------------------------------------------------------------------*/
/*	公開関数																	*/
/*------------------------------------------------------------------------------*/
/*APP_IN_CSMON_EXT void	AppCsMonInTask(void);									*//* ＣＳモニタタスク処理									*/
/*APP_IN_CSMON_EXT void	RxMsgAppCsMonIn(uint8 rcvMsgHdl);						*//* ＣＳモニタメッセージ受信通知処理						*/
/*APP_IN_CSMON_EXT uint8		GetAppCsMonInCustMsgStatus(uint8 custSigName);	*//* カスタマイズメッセージステータス取得処理				*/

#endif	/* BSW_IN_CSMON_INC */
