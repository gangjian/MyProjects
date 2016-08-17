/*	$RCSfile: share_lib_conv_step.h $									*/
/*	$Date: 2015/10/23 18:24:14JST $										*/
/*	$Revision: 1.2 $													*/
/*	 EXPLANATION: ���ʃ��C�u����_�X�e�b�v�f�[�^�ϊ� ���J�w�b�_�t�@�C��	*/

#ifndef SHARE_LIB_CONV_STEP_INC
#define SHARE_LIB_CONV_STEP_INC

#ifndef SHARE_LIB_CONV_STEP_DEF
#define SHARE_LIB_CONV_STEP_EXT extern
#else
#define SHARE_LIB_CONV_STEP_EXT
#endif

/*------------------------------------------------------------------------------*/
/*	�C���N���[�h�t�@�C��														*/
/*------------------------------------------------------------------------------*/
#include "share_lib.h"

/*------------------------------------------------------------------------------*/
/*	�ϐ���`																	*/
/*------------------------------------------------------------------------------*/

/*------------------------------------------------------------------------------*/
/*	���J��`																	*/
/*------------------------------------------------------------------------------*/

/*------------------------------------------------------------------------------*/
/*	���J�֐�																	*/
/*------------------------------------------------------------------------------*/
SHARE_LIB_CONV_STEP_EXT uint8 ShareLibConvertStep(uint8 inVal,uint8 chgTblSize,const uint8 chgTbl[],uint8 *outVal);
																				/*	�X�e�b�v�f�[�^�ϊ�����	*/

#endif	/* SHARE_LIB_CONV_STEP_INC */