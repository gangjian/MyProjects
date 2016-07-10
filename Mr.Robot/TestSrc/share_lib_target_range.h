/*	$RCSfile: share_lib_target_range.h $							*/
/*	$Date: 2015/11/06 19:13:49JST $									*/
/*	$Revision: 1.1 $												*/
/*	 EXPLANATION: 標準target(range)ライブラリ 公開ヘッダファイル	*/

#ifndef SHARE_LIB_TARGET_RANGE_INC
#define SHARE_LIB_TARGET_RANGE_INC

#ifndef SHARE_LIB_TARGET_RANGE_DEF
#define SHARE_LIB_TARGET_RANGE_EXT extern
#else
#define SHARE_LIB_TARGET_RANGE_EXT
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
	uint16 entyrInitVal;				/*	制御目標値初期値					*/
	uint16 inMax;						/*	物理値・提示情報最大値				*/
	uint16 entryValidMax;				/*	制御目標値有効最大値				*/
	uint16 entryMax;					/*	制御目標値最大値					*/
} TARGET_RANGE_MNG;

/*------------------------------------------------------------------------------*/
/*	公開関数																	*/
/*------------------------------------------------------------------------------*/
SHARE_LIB_TARGET_RANGE_EXT void ShareLibTargetRange(const TARGET_RANGE_MNG *mngTbl, uint16 inVal, uint8 funcExist, uint16 *entry);

#endif	/* SHARE_LIB_TARGET_RANGE_INC */
