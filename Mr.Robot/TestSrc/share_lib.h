/*	$RCSfile: share_lib.h $												*/
/*	$Date: 2015/10/23 13:47:25JST $										*/
/*	$Revision: 1.1 $													*/
/*	 EXPLANATION:���ʃ��C�u���� ���J�w�b�_�t�@�C��						*/

#ifndef SHARE_LIB_INC
#define SHARE_LIB_INC

#ifndef SHARE_LIB_DEF
#define SHARE_LIB_EXT extern
#else
#define SHARE_LIB_EXT
#endif

/*------------------------------------------------------------------------------*/
/*	�C���N���[�h�t�@�C��														*/
/*------------------------------------------------------------------------------*/
#include "bsw_in_csmon.h"	/* �ŏI�I�ɂ�csmon��include���g�� */

/*------------------------------------------------------------------------------*/
/*	�ϐ���`																	*/
/*------------------------------------------------------------------------------*/

/*------------------------------------------------------------------------------*/
/*	���J��`																	*/
/*------------------------------------------------------------------------------*/
/*	���C�u�������s����	*/
#define SHARE_LIB_FAIL_JUDGE_NG		((uint8)0x00)			/*	�ُ�				*/
#define SHARE_LIB_FAIL_JUDGE_OK		((uint8)0x01)			/*	����				*/

/*	�d�����	*/
#define SHARE_LIB_POWER_OFF			((uint8)0x00)			/*	�d��OFF				*/
#define SHARE_LIB_POWER_ON			((uint8)0x01)			/*	�d��ON				*/

/*	���b�Z�[�W�X�e�[�^�X	*/
#define SHARE_LIB_MSGSTS_NORMAL		(CSMON_MSGSTS_NONE)		/*	���������			*/
#define SHARE_LIB_MSGSTS_FAIL		(CSMON_MSGSTS_FAIL)		/*	�r�┭��			*/
#define SHARE_LIB_MSGSTS_NORCV 		(CSMON_MSGSTS_NORX)		/*	����M				*/

/*------------------------------------------------------------------------------*/
/*	���J�֐�																	*/
/*------------------------------------------------------------------------------*/

#endif	/* SHARE_LIB_INC */
