/*	$RCSfile: share_lib_can_in_step.h $								*/
/*	$Date: 2015/10/23 13:46:44JST $									*/
/*	$Revision: 1.1 $												*/
/*	 EXPLANATION:CAN標準入力(step)共通ライブラリ 公開ヘッダファイル	*/

#ifndef SHARE_LIB_CAN_IN_STEP_INC
#define SHARE_LIB_CAN_IN_STEP_INC

#ifndef SHARE_LIB_CAN_IN_STEP_DEF
#define SHARE_LIB_CAN_IN_STEP_EXT extern
#else
#define SHARE_LIB_CAN_IN_STEP_EXT
#endif

/*------------------------------------------------------------------------------*/
/*	インクルードファイル														*/
/*------------------------------------------------------------------------------*/
#include "share_lib.h"
#include "bsw_common_def.h"

/*------------------------------------------------------------------------------*/
/*	変数定義																	*/
/*------------------------------------------------------------------------------*/

/*------------------------------------------------------------------------------*/
/*	公開定義																	*/
/*------------------------------------------------------------------------------*/
/* 物理値状態	*/
#define SHARE_LIB_STEP_PVSTS_NORMAL			(PV_STS_NORMAL)				/*	正常						*/
#define SHARE_LIB_STEP_PVSTS_FAIL			(PV_STS_FAIL)				/*	途絶						*/
#define SHARE_LIB_STEP_PVSTS_UNFIX			(PV_STS_UNKNOWN)			/*	未確定						*/

/*	入力マネージャテーブル	*/
typedef struct{
	uint8	inSig;														/*	入力信号値					*/
	uint8	msgSts;														/*	メッセージステータス		*/
	uint8	powerSts;													/*	電源状態					*/
}VAR_IN_STEP;

/*	マネージャテーブル(VAL)	*/
typedef struct{
	uint8	chgTblSize;													/*	変換テーブル要素数			*/
	uint8	pvInitVal;													/*	物理値初期値				*/
	uint8	failVal;													/*	途絶時物理値				*/
	uint8	outRangeVal;												/*	範囲外時物理値				*/
	const uint8	*chgTbl;												/*	変換テーブルアドレス		*/
}MNG_STEP_VAL;                         

/*	マネージャテーブル(KEEP)	*/
typedef struct{
	uint8	chgTblSize;													/*	変換テーブル要素数			*/
	uint8	pvInitVal;													/*	物理値初期値				*/
	uint8	outRangeVal;												/*	範囲外時物理値				*/
	const uint8	*chgTbl;												/*	変換テーブルアドレス		*/
}MNG_STEP_KEEP;

/*	マネージャテーブル(POWERONKEEP)	*/
typedef struct{
	uint8	chgTblSize;													/*	変換テーブル要素数			*/
	uint8	failVal;													/*	途絶時物理値				*/
	uint8	outRangeVal;												/*	範囲外時物理値				*/
	const uint8	*chgTbl;												/*	変換テーブルアドレス		*/
}MNG_STEP_POWERONKEEP;

/*	出力マネージャテーブル	*/
typedef struct{
	uint8	pv;															/*	物理値						*/
	uint8	pvSts;														/*	物理値状態					*/
}VAR_OUT_STEP;

/*------------------------------------------------------------------------------*/
/*	公開関数																	*/
/*------------------------------------------------------------------------------*/
SHARE_LIB_CAN_IN_STEP_EXT void ShareLibStepFailJudgeVal(const VAR_IN_STEP *varIn, const MNG_STEP_VAL *mngData, VAR_OUT_STEP *varOut);
																		/*	CANstep共通入力関数(途絶判定VAL)			*/
SHARE_LIB_CAN_IN_STEP_EXT void ShareLibStepFailJudgeKeep(const VAR_IN_STEP *varIn, const MNG_STEP_KEEP *mngData, VAR_OUT_STEP *varOut);
																		/*	CANstep共通入力関数(途絶判定KEEP)			*/
SHARE_LIB_CAN_IN_STEP_EXT void ShareLibStepFailJudgePoweronkeep(const VAR_IN_STEP *varIn, const MNG_STEP_POWERONKEEP *mngData, VAR_OUT_STEP *varOut);
																		/*	CANstep共通入力関数(途絶判定POWERONKEEP)	*/
#endif	/* SHARE_LIB_CAN_IN_STEP_INC */
