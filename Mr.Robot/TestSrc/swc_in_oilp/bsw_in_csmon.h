/*	$RCSfile: bsw_in_csmon.h $										*/
/*	$Date: 2015/10/27 14:53:49JST $									*/
/*	$Revision: 1.2 $												*/
/*	 EXPLANATION: BSW CS���j�^ ���J�w�b�_�t�@�C��					*/

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
/*	�C���N���[�h�t�@�C��														*/
/*------------------------------------------------------------------------------*/

/*------------------------------------------------------------------------------*/
/*	���J��`																	*/
/*------------------------------------------------------------------------------*/
/*----------------------------------------------------------*/
/* �y�l�b�g���[�N�X�e�[�^�X�擾�����z						*/
/*----------------------------------------------------------*/
/* �l�b�g���[�N�X�e�[�^�X�l */
#define CSMON_NTWSTS_VALID				((uint16) 0x0000)		/* �ȉ��̃l�b�g���[�N�X�e�[�^�X��񂪗L���Ȓl�ł����Ԃ������B		*/
#define CSMON_NTWSTS_INVALID			((uint16) 0x8000)		/* �ȉ��̃l�b�g���[�N�X�e�[�^�X��񂪗L���Ȓl�łȂ���Ԃ������B		*/
#define CSMON_NTWSTS_BUSSLEEPIND		((uint16) 0x0080)		/* ���m�[�h�����M���郊���O���b�Z�[�W��Sleep.ind��Ԃ�����			*/
#define CSMON_NTWSTS_TXRINGDATAALLOWED 	((uint16) 0x0040)		/* �����O���b�Z�[�W�̈�ւ̃A�N�Z�X�֎~��Ԃ�����					*/
#define CSMON_NTWSTS_WAITBUSSLEEP		((uint16) 0x0020)		/* NMTwbsNormal����NMTwbsLimphome��Ԃ�����							*/
#define CSMON_NTWSTS_BUSSLEEP			((uint16) 0x0010)		/* NM BusSleep��Ԃ�����											*/
#define CSMON_NTWSTS_LIMPHOME			((uint16) 0x0008)		/* NM Limphome��Ԃ�����											*/
#define CSMON_NTWSTS_NMACTIVE			((uint16) 0x0004)		/* NM Active��Ԃ�����												*/
#define CSMON_NTWSTS_BUSERROR			((uint16) 0x0002)		/* �o�X�I�t��Ԃ�����												*/
#define CSMON_NTWSTS_RINGSTABLE			((uint16) 0x0001)		/* �l�b�g���[�N��Ԃ����肵�Ă���i�����O���b�Z�[�W���ꏄ���Ă��A	*/
																/* �_�������O�\���ɕύX���Ȃ��j���Ƃ����� 							*/
/* �l�b�g���[�N�X�e�[�^�X�擾���� */
/*#define CSMON_GET_NM_STATUS() 		(CsMonNetworkStatus)*/	/* �l�b�g���[�N�X�e�[�^�X���擾���܂��B								*/

/*----------------------------------------------------------*/
/* �y���b�Z�[�W�X�e�[�^�X�z									*/
/*----------------------------------------------------------*/
/* ���b�Z�[�W�X�e�[�^�X�l */
#define CSMON_MSGSTS_NONE				((uint8) 0x00)			/* ���������														*/
#define CSMON_MSGSTS_NG					((uint8) 0x80)			/* �ُ���															*/
#define CSMON_MSGSTS_FAIL				((uint8) 0x20)			/* �t�F�C��������������												*/
#define CSMON_MSGSTS_TXSTOP				((uint8) 0x10)			/* ���M�ꎞ��~���													*/
#define CSMON_MSGSTS_TIMEOUT			((uint8) 0x02) 			/* �^�C���A�E�g����													*/
#define CSMON_MSGSTS_NORX 				((uint8) 0x01)			/* ��x�����b�Z�[�W����M���Ă��Ȃ�									*/

/*----------------------------------------------------------*/
/* �y���b�Z�[�W�X�e�[�^�X�擾�����z							*/
/*----------------------------------------------------------*/
/*#define CSMON_GET_MSG_IDX_STATUS(sigName)	(CsMonMsgStatus[sigName])*/
															/* �u���b�Z�[�W�C���f�b�N�X�p�M�����v�ɂ��w�肳�ꂽ���b�Z�[�W��	*/
															/* ���b�Z�[�W�X�e�[�^�X��Ԃ��܂��B									*/

/*----------------------------------------------------------*/
/* �y���b�Z�[�W�X�e�[�^�X���o�����z							*/
/*----------------------------------------------------------*/
#define CSMON_GET_MSGSTS_BIT(msgSts,mask)		((uint8)((msgSts)&(mask)))
															/* �w�肳�ꂽ�u���b�Z�[�W�X�e�[�^�X�v���u�}�X�N�p���b�Z�[�W			*/
															/* �X�e�[�^�X�v�ɂĘ_��AND�����l��Ԃ��܂��B						*/

/*----------------------------------------------------------*/
/* �y���b�Z�[�W���菈���z									*/
/*----------------------------------------------------------*/
/*#define CSMON_TST_MSG_ST(sigName,mask)		CSMON_GET_MSGSTS_BIT(CSMON_GET_MSG_IDX_STATUS(sigName),mask)*/
															/* �u���b�Z�[�W�C���f�b�N�X�p�M�����v�ɂ��w�肳�ꂽ���b�Z�[�W		*/
															/* �X�e�[�^�X�Ɓu�}�X�N�p���b�Z�[�W�X�e�[�^�X�v�ɂĘ_��AND�����l��	*/
															/* �Ԃ��܂��B														*/

/*----------------------------------------------------------*/
/* �y�J�X�^�}�C�Y���b�Z�[�W���菈���z						*/
/*----------------------------------------------------------*/
/*#define CSMON_TST_CUST_MSG_ST(custSigName,mask)	CSMON_GET_MSGSTS_BIT(GetAppCsMonInCustMsgStatus(custSigName),mask)*/
															/* �u�J�X�^�}�C�Y���b�Z�[�W�C���f�b�N�X�p�M�����v�ɂ��w�肳�ꂽ	*/
															/* ���b�Z�[�W�X�e�[�^�X�Ɓu�}�X�N�p���b�Z�[�W�X�e�[�^�X�v�ɂĘ_��	*/
															/* AND�����l��Ԃ��܂��B											*/

/*----------------------------------------------------------*/
/* �y���b�Z�[�W��M�J�E���^�z								*/
/*----------------------------------------------------------*/
/* ���b�Z�[�W��M�J�E���^�[�l */
#define CSMON_MSG_IDX_COUNTER_MIN			((uint8) 0)			/* ���b�Z�[�W��M�J�E���^�F�ŏ��l */
#define CSMON_MSG_IDX_COUNTER_MAX			((uint8) 0xFF)		/* ���b�Z�[�W��M�J�E���^�F�ő�l */

/* ���b�Z�[�W��M�J�E���^�擾���� */
/*#define CSMON_GET_MSG_IDX_COUNTER(sigName)	(CsMonMsgRcvCnt[sigName])*/
															/* �u���b�Z�[�W�C���f�b�N�X�p�M�����v�ɂ��w�肳�ꂽ���b�Z�[�W��	*/
															/* ���b�Z�[�W��M�J�E���^��Ԃ��܂��B								*/

/*------------------------------------------------------------------------------*/
/*	���J�ϐ�																	*/
/*------------------------------------------------------------------------------*/
/*APP_IN_CSMON_EXT_C uint16	CsMonNetworkStatus;								*//* �l�b�g���[�N�X�e�[�^�X								*/
/*APP_IN_CSMON_EXT_C uint8	CsMonMsgStatus[CSMON_MSG_IDX_NUM];				*//* ���b�Z�[�W�X�e�[�^�X									*/
/*APP_IN_CSMON_EXT_C uint8	CsMonMsgRcvCnt[CSMON_MSG_IDX_NUM];				*//* ���b�Z�[�W��M�J�E���^								*/

/*------------------------------------------------------------------------------*/
/*	���J�֐�																	*/
/*------------------------------------------------------------------------------*/
/*APP_IN_CSMON_EXT void	AppCsMonInTask(void);									*//* �b�r���j�^�^�X�N����									*/
/*APP_IN_CSMON_EXT void	RxMsgAppCsMonIn(uint8 rcvMsgHdl);						*//* �b�r���j�^���b�Z�[�W��M�ʒm����						*/
/*APP_IN_CSMON_EXT uint8		GetAppCsMonInCustMsgStatus(uint8 custSigName);	*//* �J�X�^�}�C�Y���b�Z�[�W�X�e�[�^�X�擾����				*/

#endif	/* BSW_IN_CSMON_INC */
