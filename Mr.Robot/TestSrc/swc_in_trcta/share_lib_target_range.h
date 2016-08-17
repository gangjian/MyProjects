/*	$RCSfile: share_lib_target_range.h $							*/
/*	$Date: 2015/11/06 19:13:49JST $									*/
/*	$Revision: 1.1 $												*/
/*	 EXPLANATION: �W��target(range)���C�u���� ���J�w�b�_�t�@�C��	*/

#ifndef SHARE_LIB_TARGET_RANGE_INC
#define SHARE_LIB_TARGET_RANGE_INC

#ifndef SHARE_LIB_TARGET_RANGE_DEF
#define SHARE_LIB_TARGET_RANGE_EXT extern
#else
#define SHARE_LIB_TARGET_RANGE_EXT
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
	uint16 entyrInitVal;				/*	����ڕW�l�����l					*/
	uint16 inMax;						/*	�����l�E�񎦏��ő�l				*/
	uint16 entryValidMax;				/*	����ڕW�l�L���ő�l				*/
	uint16 entryMax;					/*	����ڕW�l�ő�l					*/
} TARGET_RANGE_MNG;

/*------------------------------------------------------------------------------*/
/*	���J�֐�																	*/
/*------------------------------------------------------------------------------*/
SHARE_LIB_TARGET_RANGE_EXT void ShareLibTargetRange(const TARGET_RANGE_MNG *mngTbl, uint16 inVal, uint8 funcExist, uint16 *entry);

#endif	/* SHARE_LIB_TARGET_RANGE_INC */
