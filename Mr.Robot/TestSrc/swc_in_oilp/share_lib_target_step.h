/*	$RCSfile: share_lib_target_step.h $							*/
/*	$Date: 2015/11/06 19:12:53JST $								*/
/*	$Revision: 1.2 $											*/
/*	 EXPLANATION: �W��target(step)���C�u���� ���J�w�b�_�t�@�C��	*/

#ifndef SHARE_LIB_TARGET_STEP_INC
#define SHARE_LIB_TARGET_STEP_INC

#ifndef SHARE_LIB_TARGET_STEP_DEF
#define SHARE_LIB_TARGET_STEP_EXT extern
#else
#define SHARE_LIB_TARGET_STEP_EXT
#endif

/*------------------------------------------------------------------------------*/
/*	�C���N���[�h�t�@�C��														*/
/*------------------------------------------------------------------------------*/
#include "share_lib.h"

/*------------------------------------------------------------------------------*/
/*	���J��`																	*/
/*------------------------------------------------------------------------------*/
/*	�@�\�L��	*/
#ifndef SHARE_LIB_FUNCSTS_INVALID
#define SHARE_LIB_FUNCSTS_INVALID	((uint8) 0U)	/*	�@�\����				*/
#endif /* SHARE_LIB_FUNCSTS_INVALID */

#ifndef SHARE_LIB_FUNCSTS_VALID
#define SHARE_LIB_FUNCSTS_VALID		((uint8) 1U)	/*	�@�\�L��				*/
#endif /* SHARE_LIB_FUNCSTS_VALID */

/*------------------------------------------------------------------------------*/
/*	�\���̒�`																	*/
/*------------------------------------------------------------------------------*/
/* �}�l�[�W���e�[�u�� */
typedef struct{
	uint8 chgTblSize;					/*	�ϊ��e�[�u���v�f��					*/
	const uint8 *chgTbl;				/*	�ϊ��e�[�u���A�h���X				*/
	uint8 entryInitVal;					/*	����ڕW�l�����l					*/
	uint8 outRangeVal;					/*	�͈͊O������ڕW�l					*/
}TARGET_STEP_MNG;

/*------------------------------------------------------------------------------*/
/*	���J�֐�																	*/
/*------------------------------------------------------------------------------*/
SHARE_LIB_TARGET_STEP_EXT void ShareLibTargetStep(const TARGET_STEP_MNG *mngTbl, uint8 inVal, uint8 funcExist, uint8 *entry);

#endif /* SHARE_LIB_TARGET_STEP_INC */

