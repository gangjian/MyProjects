/*	$RCSfile: bsw_common_nvdef.h $									*/
/*	$Date: 2015/11/04 20:36:59JST $									*/
/*	$Revision: 1.5 $												*/
/*	 EXPLANATION: BSW���ʒ�`(�s������������) ���J�w�b�_�t�@�C��	*/

#ifndef BSW_COMMON_NVDEF_INC
#define BSW_COMMON_NVDEF_INC

#ifndef BSW_COMMON_NVDEF_DEF
#define BSW_COMMON_NVDEF_EXT extern
#else
#define BSW_COMMON_NVDEF_EXT
#endif

/*	TBC�\���@�\�L��	*/
#define NV_TTBC_FUNC_STS_INVALID		((uint8) 0U)			/*	TBC�\���@�\����						*/
#define NV_TTBC_FUNC_STS_VALID			((uint8) 1U)			/*	TBC�\���@�\�L��						*/

/*	LDA�V�X�e���L��	*/
#define NV_KINOUM_LDA_OFF				((uint8) 0U)			/*	LDA�V�X�e������						*/
#define NV_KINOUM_LDA_ON				((uint8) 1U)			/*	LDA�V�X�e���L��						*/

/*	�^�C����C���V�X�e���L��	*/
#define NV_KINOUM_TPMS_OFF				((uint8) 0U)			/*	�^�C����C���V�X�e������			*/
#define NV_KINOUM_TPMS_ON				((uint8) 1U)			/*	�^�C����C���V�X�e���L��			*/

/*	�V�[�g�x���g�u�U�[�L�����Z��	*/
#define NV_BKLBZ_NONACTIVE				((uint8) 0U)			/*	�V�[�g�x���g�u�U�[�@�\NonActive		*/
#define NV_BKLBZ_ACTIVE					((uint8) 1U)			/*	�V�[�g�x���g�u�U�[�@�\Active		*/

/*	�\�t�gSW(RCTA)�V�X�e���L��	*/
#define NV_KINOUM_RCTA_OFF				((uint8) 0U)			/*	�\�t�gSW(RCTA)�V�X�e������			*/
#define NV_KINOUM_RCTA_ON				((uint8) 1U)			/*	�\�t�gSW(RCTA)�V�X�e���L��			*/

/*	BSM�V�X�e���L��	*/
#define NV_KINOUM_BSM_OFF				((uint8) 0U)			/*	BSM�V�X�e������						*/
#define NV_KINOUM_BSM_ON				((uint8) 1U)			/*	BSM�V�X�e���L��						*/

/*	PCS�V�X�e���L��	*/
#define NV_KINOUM_PCS_OFF				((uint8) 0U)			/*	PCS�V�X�e������						*/
#define NV_KINOUM_PCS_ON				((uint8) 1U)			/*	PCS�V�X�e���L��						*/

/*	TPMS�\���L��	*/
#define NV_TPMS_DISP_INVALID			((uint8) 0U)			/*	TPMS�\���L��						*/
#define NV_TPMS_DISP_VALID				((uint8) 1U)			/*	TPMS�\������						*/

/*	EHW2�E�F�C�N�A�b�v�v��	*/
#define NV_WAKEUP_FACT_OFF				((uint8) 0U)			/*	�v������							*/
#define NV_WAKEUP_FACT_ON				((uint8) 1U)			/*	�v���L��							*/

/*	�n�U�[�h�_�����������	*/
#define NV_HZDMEM_OFF					((uint16) 0U)			/*	�n�U�[�hOFF							*/
#define NV_HZDMEM_ON					((uint16) 65535U)		/*	���[�����^��SW ON					*/
#define NV_HZDMEM_UNKNOWN				((uint16) 43690U)		/*	����`								*/
#define NV_HZDMEM_RSCARFIND_ON			((uint16) 65533U)		/*	RS�J�[�t�@�C���_�v�� ON				*/
#define NV_HZDMEM_RSHZD_ON				((uint16) 65534U)		/*	RS�n�U�[�h�v�� ON					*/

/*	�^�[���n�U�[�h�V���[�g�E�f���EDTC���	*/
#define NV_TNHZD_LAMP_STS_NORMAL		((uint8) 0U)			/*	����								*/
#define NV_TNHZD_LAMP_STS_SHORT			((uint8) 1U)			/*	�V���[�g							*/
#define NV_TNHZD_LAMP_STS_BREAK			((uint8) 2U)			/*	�f��								*/
#define NV_TNHZD_LAMP_STS_DTC_SHORT		((uint8) 4U)			/*	�V���[�gDTC							*/
#define NV_TNHZD_LAMP_STS_DTC_BREAK		((uint8) 8U)			/*	�f��DTC								*/

/*	��r�f�[�^���	*/
#define NV_FUEL_DATSTS_NG				((uint8) 0U)
#define NV_FUEL_DATSTS_OK				((uint8) 1U)

/*	����������	*/
#define NV_FUEL_REFSTS_FALSE			((uint8) 0U)
#define NV_FUEL_REFSTS_TRUE				((uint8) 1U)

/*	VSC����L��	*/
#define NV_KINOUM_VSC_OFF				((uint8) 0U)			/*	VSC���䖳��							*/
#define NV_KINOUM_VSC_ON				((uint8) 1U)			/*	VSC����L��							*/

/*	�u�U�[�v��	*/
#define NV_BZ_OFF						((uint8) 0U)			/*	������							*/
#define NV_BZ_ON						((uint8) 1U)			/*	���o�^							*/
#define NV_BZ_CYCLEOFF					((uint8) 2U)			/*	1�����㐁����						*/

/*	NV_�u�U�[���	*/
#define NV_BZ_STATE_STOP				((uint8) 0U)			/*	�u�U�[��ԁF��~					*/
#define NV_BZ_STATE_WAIT				((uint8) 1U)			/*	�u�U�[��ԁF���҂�				*/
#define NV_BZ_STATE_RUN					((uint8) 2U)			/*	�u�U�[��ԁF����					*/
#define NV_BZ_STATE_FINISHED			((uint8) 3U)			/*	�u�U�[��ԁF����					*/
#define NV_BZ_STATE_CYCLESTOP			((uint8) 4U)			/*	�u�U�[��ԁF1������OFF				*/

/*	�����X�e�b�v�l(�񌸌�)	*/
#define NV_ILLCNT_STEP1D				((uint8) 0U)			/*	�񌸌����[�hSTEP1					*/
#define NV_ILLCNT_STEP2D				((uint8) 1U)			/*	�񌸌����[�hSTEP2					*/
#define NV_ILLCNT_STEP3D				((uint8) 2U)			/*	�񌸌����[�hSTEP3					*/
#define NV_ILLCNT_STEP4D				((uint8) 3U)			/*	�񌸌����[�hSTEP4					*/
#define NV_ILLCNT_STEP5D				((uint8) 4U)			/*	�񌸌����[�hSTEP5					*/
#define NV_ILLCNT_STEP6D				((uint8) 5U)			/*	�񌸌����[�hSTEP6					*/
#define NV_ILLCNT_STEP7D				((uint8) 6U)			/*	�񌸌����[�hSTEP7					*/
#define NV_ILLCNT_STEP8D				((uint8) 7U)			/*	�񌸌����[�hSTEP8					*/
#define NV_ILLCNT_STEP9D				((uint8) 8U)			/*	�񌸌����[�hSTEP9					*/
#define NV_ILLCNT_STEP10D				((uint8) 9U)			/*	�񌸌����[�hSTEP10					*/
#define NV_ILLCNT_STEP11D				((uint8) 10U)			/*	�񌸌����[�hSTEP11					*/
#define NV_ILLCNT_STEP12D				((uint8) 11U)			/*	�񌸌����[�hSTEP12					*/
#define NV_ILLCNT_STEP13D				((uint8) 12U)			/*	�񌸌����[�hSTEP13					*/
#define NV_ILLCNT_STEP14D				((uint8) 13U)			/*	�񌸌����[�hSTEP14					*/
#define NV_ILLCNT_STEP15D				((uint8) 14U)			/*	�񌸌����[�hSTEP15					*/
#define NV_ILLCNT_STEP16D				((uint8) 15U)			/*	�񌸌����[�hSTEP16					*/
#define NV_ILLCNT_STEP17D				((uint8) 16U)			/*	�񌸌����[�hSTEP17					*/
#define NV_ILLCNT_STEP18D				((uint8) 17U)			/*	�񌸌����[�hSTEP18					*/
#define NV_ILLCNT_STEP19D				((uint8) 18U)			/*	�񌸌����[�hSTEP19					*/
#define NV_ILLCNT_STEP20D				((uint8) 19U)			/*	�񌸌����[�hSTEP20					*/

/*	�����X�e�b�v�l(����)	*/
#define NV_ILLCNT_STEP1N				((uint8) 0U)			/*	�������[�hSTEP1						*/
#define NV_ILLCNT_STEP2N				((uint8) 1U)			/*	�������[�hSTEP2						*/
#define NV_ILLCNT_STEP3N				((uint8) 2U)			/*	�������[�hSTEP3						*/
#define NV_ILLCNT_STEP4N				((uint8) 3U)			/*	�������[�hSTEP4						*/
#define NV_ILLCNT_STEP5N				((uint8) 4U)			/*	�������[�hSTEP5						*/
#define NV_ILLCNT_STEP6N				((uint8) 5U)			/*	�������[�hSTEP6						*/
#define NV_ILLCNT_STEP7N				((uint8) 6U)			/*	�������[�hSTEP7						*/
#define NV_ILLCNT_STEP8N				((uint8) 7U)			/*	�������[�hSTEP8						*/
#define NV_ILLCNT_STEP9N				((uint8) 8U)			/*	�������[�hSTEP9						*/
#define NV_ILLCNT_STEP10N				((uint8) 9U)			/*	�������[�hSTEP10					*/
#define NV_ILLCNT_STEP11N				((uint8) 10U)			/*	�������[�hSTEP11					*/
#define NV_ILLCNT_STEP12N				((uint8) 11U)			/*	�������[�hSTEP12					*/
#define NV_ILLCNT_STEP13N				((uint8) 12U)			/*	�������[�hSTEP13					*/
#define NV_ILLCNT_STEP14N				((uint8) 13U)			/*	�������[�hSTEP14					*/
#define NV_ILLCNT_STEP15N				((uint8) 14U)			/*	�������[�hSTEP15					*/
#define NV_ILLCNT_STEP16N				((uint8) 15U)			/*	�������[�hSTEP16					*/
#define NV_ILLCNT_STEP17N				((uint8) 16U)			/*	�������[�hSTEP17					*/
#define NV_ILLCNT_STEP18N				((uint8) 17U)			/*	�������[�hSTEP18					*/
#define NV_ILLCNT_STEP19N				((uint8) 18U)			/*	�������[�hSTEP19					*/
#define NV_ILLCNT_STEP20N				((uint8) 19U)			/*	�������[�hSTEP20					*/

/*	���E���W�C���~�����M��	*/
#define NV_ILLOUT_ILLOF_ON				((uint8) 0U)			/*	�_���w��							*/
#define NV_ILLOUT_ILLOF_OFF				((uint8) 1U)			/*	�����w��							*/

/*	NV�I�h�g���\�����	*/
#define	NV_ODTR_DISPSTS_ODO				((uint8) 0U)			/*	ODO�\��								*/
#define	NV_ODTR_DISPSTS_TRIPA			((uint8) 1U)			/*	TRIPA�\��							*/
#define	NV_ODTR_DISPSTS_TRIPB			((uint8) 2U)			/*	TRIPB�\��							*/

/*	NV�I�h�ώZ�p���X	*/
#define	NV_ODO_PULSE_MIN				((uint16) 0U)			/*	�ŏ��I�h�ώZ�p���X					*/
#define	NV_ODO_PULSE_MAX				((uint16) 65535U)		/*	�ő�I�h�ώZ�p���X					*/

/*	NV�g���b�vA�ώZ�p���X	*/
#define	NV_TRIPA_PULSE_MIN				((uint16) 0U)			/*	�ŏ��g���b�vA�ώZ�p���X				*/
#define	NV_TRIPA_PULSE_MAX				((uint16) 65535U)		/*	�ő�g���b�vA�ώZ�p���X				*/

/*	NV�g���b�vB�ώZ�p���X	*/
#define	NV_TRIPB_PULSE_MIN				((uint16) 0U)			/*	�ŏ��g���b�vB�ώZ�p���X				*/
#define	NV_TRIPB_PULSE_MAX				((uint16) 65535U)		/*	�ő�g���b�vB�ώZ�p���X				*/

/*	NV�I�h�P�ʏ��	*/
#define	NV_ODO_UNIT_KM					((uint8) 0U)			/*	�I�h�P�ʁFKM						*/
#define	NV_ODO_UNIT_MILE				((uint8) 1U)			/*	�I�h�P�ʁFMILE						*/

/*	NV�I�h�\���l�P�ʏ��	*/
#define	NV_ODO_DISP_UNIT_KM				((uint8) 0U)			/*	�I�h�\���l�P�ʁFKM					*/
#define	NV_ODO_DISP_UNIT_MILE			((uint8) 1U)			/*	�I�h�\���l�P�ʁFMILE				*/

/*	NV�[���p���X�������`�F�b�N	*/
#define	NV_CHKPLS_MIN					((uint16) 0U)			/*	�[���p���X�ŏ��l					*/
#define	NV_CHKPLS_MAX					((uint16) 5095U)		/*	�[���p���X�ő�l					*/
#define	NV_CHKPLS_FALSE					((uint16) 0xAA55U)		/*	�͈͊O�f�[�^						*/

/*	�h���C�u���j�^�P�ʐؑ֏��	*/
#define	NV_MCUST_UNIT_NOT_SET			((uint8) 0U)			/*	���ݒ�								*/
#define	NV_MCUST_UNIT_KML				((uint8) 1U)			/*	km/L								*/
#define	NV_MCUST_UNIT_L100KM			((uint8) 2U)			/*	L/100km								*/
#define	NV_MCUST_UNIT_MPG_US			((uint8) 3U)			/*	MPG(US)								*/
#define	NV_MCUST_UNIT_MPG_UK			((uint8) 4U)			/*	MPG(UK)								*/
#define	NV_MCUST_UNIT_KMKG				((uint8) 5U)			/*	km/kg								*/
#define	NV_MCUST_UNIT_KMGALLON			((uint8) 6U)			/*	km/gallon							*/

/*	NV�I�h�l					*/
#define	NV_ODO_MIN						((uint32) 0U)			/*	�ŏ��I�h�l							*/
#define	NV_ODO_MAX						((uint32) 999999U)		/*	�ő�I�h�l							*/

/*	NV�g���b�v�lA				*/
#define	NV_TRIPA_MIN					((uint32) 0U)			/*	�ŏ��g���b�vA�l						*/
#define	NV_TRIPA_MAX					((uint32) 99999U)		/*	�ő�g���b�vA�l						*/

/*	NV�g���b�v�lB				*/
#define	NV_TRIPB_MIN					((uint32) 0U)			/*	�ŏ��g���b�vB�l						*/
#define	NV_TRIPB_MAX					((uint32) 99999U)		/*	�ő�g���b�vB�l						*/

#endif	/* BSW_COMMON_NVDEF_INC */

