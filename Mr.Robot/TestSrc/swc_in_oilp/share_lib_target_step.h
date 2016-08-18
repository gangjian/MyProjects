/*	$RCSfile: share_lib_target_step.h $							*/
/*	$Date: 2015/11/06 19:12:53JST $								*/
/*	$Revision: 1.2 $											*/
/*	 EXPLANATION: 標準target(step)ライブラリ 公開ヘッダファイル	*/

#ifndef SHARE_LIB_TARGET_STEP_INC
#define SHARE_LIB_TARGET_STEP_INC

#ifndef SHARE_LIB_TARGET_STEP_DEF
#define SHARE_LIB_TARGET_STEP_EXT extern
#else
#define SHARE_LIB_TARGET_STEP_EXT
#endif

/*------------------------------------------------------------------------------*/
/*	インクルードファイル														*/
/*------------------------------------------------------------------------------*/
#include "share_lib.h"

/*------------------------------------------------------------------------------*/
/*	公開定義																	*/
/*------------------------------------------------------------------------------*/
/*	機能有無	*/
#ifndef SHARE_LIB_FUNCSTS_INVALID
#define SHARE_LIB_FUNCSTS_INVALID	((uint8) 0U)	/*	機能無し				*/
#endif /* SHARE_LIB_FUNCSTS_INVALID */

#ifndef SHARE_LIB_FUNCSTS_VALID
#define SHARE_LIB_FUNCSTS_VALID		((uint8) 1U)	/*	機能有り				*/
#endif /* SHARE_LIB_FUNCSTS_VALID */

/*------------------------------------------------------------------------------*/
/*	構造体定義																	*/
/*------------------------------------------------------------------------------*/
/* マネージャテーブル */
typedef struct{
	uint8 chgTblSize;					/*	変換テーブル要素数					*/
	const uint8 *chgTbl;				/*	変換テーブルアドレス				*/
	uint8 entryInitVal;					/*	制御目標値初期値					*/
	uint8 outRangeVal;					/*	範囲外時制御目標値					*/
}TARGET_STEP_MNG;

/*------------------------------------------------------------------------------*/
/*	公開関数																	*/
/*------------------------------------------------------------------------------*/
SHARE_LIB_TARGET_STEP_EXT void ShareLibTargetStep(const TARGET_STEP_MNG *mngTbl, uint8 inVal, uint8 funcExist, uint8 *entry);

#endif /* SHARE_LIB_TARGET_STEP_INC */

