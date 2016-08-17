/*	$RCSfile: share_lib.h $												*/
/*	$Date: 2015/10/23 13:47:25JST $										*/
/*	$Revision: 1.1 $													*/
/*	 EXPLANATION:共通ライブラリ 公開ヘッダファイル						*/

#ifndef SHARE_LIB_INC
#define SHARE_LIB_INC

#ifndef SHARE_LIB_DEF
#define SHARE_LIB_EXT extern
#else
#define SHARE_LIB_EXT
#endif

/*------------------------------------------------------------------------------*/
/*	インクルードファイル														*/
/*------------------------------------------------------------------------------*/
#include "bsw_in_csmon.h"	/* 最終的にはcsmonのincludeを使う */

/*------------------------------------------------------------------------------*/
/*	変数定義																	*/
/*------------------------------------------------------------------------------*/

/*------------------------------------------------------------------------------*/
/*	公開定義																	*/
/*------------------------------------------------------------------------------*/
/*	ライブラリ実行結果	*/
#define SHARE_LIB_FAIL_JUDGE_NG		((uint8)0x00)			/*	異常				*/
#define SHARE_LIB_FAIL_JUDGE_OK		((uint8)0x01)			/*	正常				*/

/*	電源状態	*/
#define SHARE_LIB_POWER_OFF			((uint8)0x00)			/*	電源OFF				*/
#define SHARE_LIB_POWER_ON			((uint8)0x01)			/*	電源ON				*/

/*	メッセージステータス	*/
#define SHARE_LIB_MSGSTS_NORMAL		(CSMON_MSGSTS_NONE)		/*	未発生状態			*/
#define SHARE_LIB_MSGSTS_FAIL		(CSMON_MSGSTS_FAIL)		/*	途絶発生			*/
#define SHARE_LIB_MSGSTS_NORCV 		(CSMON_MSGSTS_NORX)		/*	未受信				*/

/*------------------------------------------------------------------------------*/
/*	公開関数																	*/
/*------------------------------------------------------------------------------*/

#endif	/* SHARE_LIB_INC */
