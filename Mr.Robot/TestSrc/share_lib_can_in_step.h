/*	$RCSfile: share_lib_can_in_step.h $								*/
/*	$Date: 2015/10/23 13:46:44JST $									*/
/*	$Revision: 1.1 $												*/
/*	 EXPLANATION:CAN�W������(step)���ʃ��C�u���� ���J�w�b�_�t�@�C��	*/

#ifndef SHARE_LIB_CAN_IN_STEP_INC
#define SHARE_LIB_CAN_IN_STEP_INC

#ifndef SHARE_LIB_CAN_IN_STEP_DEF
#define SHARE_LIB_CAN_IN_STEP_EXT extern
#else
#define SHARE_LIB_CAN_IN_STEP_EXT
#endif

/*------------------------------------------------------------------------------*/
/*	�C���N���[�h�t�@�C��														*/
/*------------------------------------------------------------------------------*/
#include "share_lib.h"
#include "bsw_common_def.h"

/*------------------------------------------------------------------------------*/
/*	�ϐ���`																	*/
/*------------------------------------------------------------------------------*/

/*------------------------------------------------------------------------------*/
/*	���J��`																	*/
/*------------------------------------------------------------------------------*/
/* �����l���	*/
#define SHARE_LIB_STEP_PVSTS_NORMAL			(PV_STS_NORMAL)				/*	����						*/
#define SHARE_LIB_STEP_PVSTS_FAIL			(PV_STS_FAIL)				/*	�r��						*/
#define SHARE_LIB_STEP_PVSTS_UNFIX			(PV_STS_UNKNOWN)			/*	���m��						*/

/*	���̓}�l�[�W���e�[�u��	*/
typedef struct{
	uint8	inSig;														/*	���͐M���l					*/
	uint8	msgSts;														/*	���b�Z�[�W�X�e�[�^�X		*/
	uint8	powerSts;													/*	�d�����					*/
}VAR_IN_STEP;

/*	�}�l�[�W���e�[�u��(VAL)	*/
typedef struct{
	uint8	chgTblSize;													/*	�ϊ��e�[�u���v�f��			*/
	uint8	pvInitVal;													/*	�����l�����l				*/
	uint8	failVal;													/*	�r�⎞�����l				*/
	uint8	outRangeVal;												/*	�͈͊O�������l				*/
	const uint8	*chgTbl;												/*	�ϊ��e�[�u���A�h���X		*/
}MNG_STEP_VAL;                         

/*	�}�l�[�W���e�[�u��(KEEP)	*/
typedef struct{
	uint8	chgTblSize;													/*	�ϊ��e�[�u���v�f��			*/
	uint8	pvInitVal;													/*	�����l�����l				*/
	uint8	outRangeVal;												/*	�͈͊O�������l				*/
	const uint8	*chgTbl;												/*	�ϊ��e�[�u���A�h���X		*/
}MNG_STEP_KEEP;

/*	�}�l�[�W���e�[�u��(POWERONKEEP)	*/
typedef struct{
	uint8	chgTblSize;													/*	�ϊ��e�[�u���v�f��			*/
	uint8	failVal;													/*	�r�⎞�����l				*/
	uint8	outRangeVal;												/*	�͈͊O�������l				*/
	const uint8	*chgTbl;												/*	�ϊ��e�[�u���A�h���X		*/
}MNG_STEP_POWERONKEEP;

/*	�o�̓}�l�[�W���e�[�u��	*/
typedef struct{
	uint8	pv;															/*	�����l						*/
	uint8	pvSts;														/*	�����l���					*/
}VAR_OUT_STEP;

/*------------------------------------------------------------------------------*/
/*	���J�֐�																	*/
/*------------------------------------------------------------------------------*/
SHARE_LIB_CAN_IN_STEP_EXT void ShareLibStepFailJudgeVal(const VAR_IN_STEP *varIn, const MNG_STEP_VAL *mngData, VAR_OUT_STEP *varOut);
																		/*	CANstep���ʓ��͊֐�(�r�┻��VAL)			*/
SHARE_LIB_CAN_IN_STEP_EXT void ShareLibStepFailJudgeKeep(const VAR_IN_STEP *varIn, const MNG_STEP_KEEP *mngData, VAR_OUT_STEP *varOut);
																		/*	CANstep���ʓ��͊֐�(�r�┻��KEEP)			*/
SHARE_LIB_CAN_IN_STEP_EXT void ShareLibStepFailJudgePoweronkeep(const VAR_IN_STEP *varIn, const MNG_STEP_POWERONKEEP *mngData, VAR_OUT_STEP *varOut);
																		/*	CANstep���ʓ��͊֐�(�r�┻��POWERONKEEP)	*/
#endif	/* SHARE_LIB_CAN_IN_STEP_INC */
