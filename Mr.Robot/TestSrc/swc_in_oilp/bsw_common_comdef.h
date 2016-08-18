/*	$RCSfile: bsw_common_comdef.h $									*/
/*	$Date: 2015/11/09 13:26:54JST $									*/
/*	$Revision: 1.4 $												*/
/*	 EXPLANATION: BSW���ʒ�`(����ڕW�l(com)) ���J�w�b�_�t�@�C��	*/

#ifndef BSW_COMMON_COMDEF_INC
#define BSW_COMMON_COMDEF_INC

#ifndef BSW_COMMON_COMDEF_DEF
#define BSW_COMMON_COMDEF_EXT extern
#else
#define BSW_COMMON_COMDEF_EXT
#endif

/*------------------------------------------------------------------------------*/
/*	�}�N����`(�ύX�֎~)														*/
/*------------------------------------------------------------------------------*/
/* �M���L���ݒ� */
#define COMMON_COMDEF_SIG_NONEXIST		(0)								/* ���ʕʂ̐M�����L���F����				*/
#define COMMON_COMDEF_SIG_EXIST			(1)								/* ���ʕʂ̐M�����L���F�L��				*/

/*----------------------------------------------------------*/
/* �M��ID�L���i�ύX�j										*/
/* REMOTE_UINT8,uint8,uint16,uint32���ɐM��ID�L����ݒ�		*/
/*----------------------------------------------------------*/
#define COMMON_COMDEF_SIG_RW_UINT8		(COMMON_COMDEF_SIG_NONEXIST)	/* �����[�g�E�H�[�j���O�M��ID�L��			*/
#define COMMON_COMDEF_SIG_UINT8			(COMMON_COMDEF_SIG_EXIST)		/* ����uint8�̐M��ID�L��					*/
#define COMMON_COMDEF_SIG_UINT16		(COMMON_COMDEF_SIG_EXIST)		/* ����uint16�̐M��ID�L��					*/
#define COMMON_COMDEF_SIG_UINT32		(COMMON_COMDEF_SIG_EXIST)		/* ����uint32�̐M��ID�L��					*/


/********************/
/*	�e�f�[�^��`	*/
/********************/
/*------------------------------------------*/
/*	�ʏ퐧��ڕW�l�p��`					*/
/*	�A�N�e�B�u�e�X�g����ڕW�l�p��`		*/
/*------------------------------------------*/
/*	�����[�g�E�H�[�j���O����ڕW�l	*/
#ifndef	APP_UNUSED_OUT_SIG_RW_ALL
#define TARGET_COM_DT_RW_OFF				((uint8) 0U)		/*	�\���v����										*/
#define TARGET_COM_DT_RW_ON					((uint8) 1U)		/*	�\���v���L										*/
#endif	/*	APP_UNUSED_OUT_SIG_RW_ALL	*/

/* �����[�U�[�ҏW�\�� */

/*	�|�[�����O���M�v������ڕW(METD_MST)	*/
#define TARGET_COM_DT_DIAGCOM_TARGET_NG		((uint8) 0U)		/*	�|�[�����O���M�v��:��							*/
#define TARGET_COM_DT_DIAGCOM_TARGET_NORMAL	((uint8) 1U)		/*	�|�[�����O���M�v��:�ʏ�_�C�A�O					*/
#define TARGET_COM_DT_DIAGCOM_TARGET_CMPL	((uint8) 2U)		/*	�|�[�����O���M�v��:�����_�C�A�O					*/

/*	���M�ԑ�����ڕW�l(MET_SPD)	*/
#define TARGET_COM_DT_SPD_INIT				((uint8) 0U)		/*	���M�ԑ������l									*/

/*	���݋P�x�l(���I�X�^�b�g)����ڕW�l(RHEOSTAT)	*/
#define TARGET_COM_DT_ILL_RHEOS_DUTY_MIN	((uint8) 0U)		/*	���݋P�x�l(���I�X�^�b�g)�̍ŏ��l(LSB:1(%))		*/
#define TARGET_COM_DT_ILL_RHEOS_DUTY_MAX	((uint8) 100U)		/*	���݋P�x�l(���I�X�^�b�g)�̍ő�l(LSB:1(��))		*/

/*	�e�[���L�����Z���M������ڕW�l(TAIL_CN)	*/
#define TARGET_COM_DT_TAIL_CN_UNKNOWN		((uint8) 0U)		/*	SW��ԕs�m��									*/
#define TARGET_COM_DT_TAIL_CN_UNFIX			((uint8) 1U)		/*	����`											*/
#define TARGET_COM_DT_TAIL_CN_ON			((uint8) 2U)		/*	�e�[���L�����Z�����(ON)						*/
#define TARGET_COM_DT_TAIL_CN_OFF			((uint8) 3U)		/*	��e�[���L�����Z�����(OFF)						*/

/*	���E���W�C���~�����M������ڕW�l(ILL_OF)	*/
#define TARGET_COM_DT_ILL_OF_ON				((uint8) 0U)		/*	�_���w��										*/
#define TARGET_COM_DT_ILL_OF_OFF			((uint8) 1U)		/*	�����w��										*/

/*	�O�C���x���M�P�ʐ���ڕW�l(UNIT_TMP)	*/
#define TARGET_COM_DT_OSTEMP2_TARGET_COMUNIT_UNKNOWN1	((uint8) 0U)	/* ���m��1									*/
#define TARGET_COM_DT_OSTEMP2_TARGET_COMUNIT_UNKNOWN2	((uint8) 0U)	/* ���m��2									*/
#define TARGET_COM_DT_OSTEMP2_TARGET_COMUNIT_CNTGRD		((uint8) 0U)	/* �ێ�(��)									*/
#define TARGET_COM_DT_OSTEMP2_TARGET_COMUNIT_FHRNHT		((uint8) 0U)	/* �؎�(�KF)								*/

/*	�h���C�u���j�^�P�ʐM��0����ڕW�l(UNIT_0)	*/
/*	��`����								*/

/*	�X�s�[�h������񐧌�ڕW�l(SP_TL)	*/
#define TARGET_COM_DT_TOLER_UNDEF			((uint8) 0U)		/*	����`											*/

/*	�d���n��񐧌�ڕW�l(MET_DEST)	*/
#define TARGET_COM_DT_DEST_NON				((uint8) 240U)		/*	�d���n��񖳂�									*/

/*	�n���h����񐧌�ڕW�l(R_L)	*/
#define TARGET_COM_DT_HANDLE_NON			((uint8) 0U)		/* �n���h����񖳂�									*/
																/* 	(�ʐM�������AEEPROM�̏᎞�A�n���h�����E���ʎ�)	*/
#define TARGET_COM_DT_HANDLE_R				((uint8) 1U)		/* �E�n���h����										*/
#define TARGET_COM_DT_HANDLE_L				((uint8) 2U)		/* ���n���h����										*/
#define TARGET_COM_DT_HANDLE_UNDEF			((uint8) 3U)		/* �n���h����񖳂�(����`)							*/

/*	�I�h�P��(�ʐM)����ڕW�l(ODO_UNIT)	*/
#define	TARGET_COM_DT_ODTR_ODO_UNIT_COM_UNKNOWN			((uint8) 0U)	/*	�I�h���M�P�ʁF�P�ʏ�񖳂�				*/
#define	TARGET_COM_DT_ODTR_ODO_UNIT_COM_KM				((uint8) 1U)	/*	�I�h���M�P�ʁFKM						*/
#define	TARGET_COM_DT_ODTR_ODO_UNIT_COM_MILE			((uint8) 2U)	/*	�I�h���M�P�ʁFMILE						*/
#define	TARGET_COM_DT_ODTR_ODO_UNIT_COM_UNKNOWN2		((uint8) 3U)	/*	�I�h���M�P�ʁF�P�ʏ�񖳂�				*/

/*	�h���C�u���j�^�P�ʐM��1����ڕW�l(UNIT_1)	*/
/*	��`����								*/

/*	�h���C�u���j�^�P�ʐM��2����ڕW�l(UNIT_2)	*/
/*	��`����								*/

/*	�h���C�u���j�^�P�ʐM��3����ڕW�l(UNIT_3)	*/
/*	��`����								*/

/*	�R��\���X�P�[��(FC_SCL)	*/
#define TARGET_COM_DT_FC_SCL_UNKNOWN		((uint8)0x00)		/*	����`											*/
#define TARGET_COM_DT_FC_SCL_15KPL			((uint8)0x01)		/*	15km/L											*/
#define TARGET_COM_DT_FC_SCL_20KPL			((uint8)0x02)		/*	20km/L											*/
#define TARGET_COM_DT_FC_SCL_30KPL			((uint8)0x03)		/*	30km/L 1										*/
#define TARGET_COM_DT_FC_SCL_40KPL			((uint8)0x04)		/*	40km/L											*/
#define TARGET_COM_DT_FC_SCL_30KPL2			((uint8)0x05)		/*	30km/L 2										*/
#define TARGET_COM_DT_FC_SCL_UNKNOWN2		((uint8)0x06)		/*	����`2											*/
#define TARGET_COM_DT_FC_SCL_UNKNOWN3		((uint8)0x07)		/*	����`3											*/

/*	���M�ԑ�����ڕW�l(�Q�[�W)	*/
#define TARGET_COM_DT_SP_MIN				((uint8) 0U)		/* ���M�ԑ��ŏ��l									*/
#define TARGET_COM_DT_SP_MAX				((uint8) 255U)		/* ���M�ԑ��ő�l									*/


/*	�h���C�u���j�^�P�ʐM��4����ڕW�l(UNIT_4)	*/
/*	��`����								*/

/*	�h���C�u���j�^�P�ʐM��5����ڕW�l(UNIT_5)	*/
/*	��`����								*/

/*	�h���C�u���j�^�P�ʐM��6����ڕW�l(UNIT_6)	*/
/*	��`����								*/

/*	�t���[�G���Q�[�W��Ԑ���ڕW�l(FUGAGE_S)	*/
#define TARGET_COM_DT_FUGAGES_UNFIX 		((uint8) 0U)		/* ���m��											*/
#define TARGET_COM_DT_FUGAGES_FIXED			((uint8) 1U)		/* �m��												*/

/*	�t���[�G���Q�[�W�w���l����ڕW�l(FUGAGE)	*/
#define TARGET_COM_DT_FUGAGE_MIN			((uint8) 0U)		/*	Fuel�Q�[�W�w���l�M���ŏ��l						*/
#define TARGET_COM_DT_FUGAGE_MAX			((uint8) 255U)		/*	Fuel�Q�[�W�w���l�M���ő�l						*/

/*	�I�C�������e�i���X��������ڕW�l(OM_MLG)	*/
/*	��`����									*/

/*	�v���I�C�������e�i���X�t���O����ڕW�l(PR_OM_FL)	*/
/*	��`����											*/

/*	�^�[�������v��Ԑ���ڕW�l(TNS)	*/
#define TARGET_COM_DT_TNS_UNDEF				((uint8) 0U)		/*	����`											*/
#define TARGET_COM_DT_TNS_TURNL				((uint8) 1U)		/*	�^�[�������vL���쒆								*/
#define TARGET_COM_DT_TNS_TURNR				((uint8) 2U)		/*	�^�[�������vR���쒆								*/
#define TARGET_COM_DT_TNS_OFF				((uint8) 3U)		/*	�^�[�������v�񓮍�								*/

/*	�n�U�[�h���͏�Ԑ���ڕW�l(HZS)	*/
#define TARGET_COM_DT_HZS_OFF				((uint8) 0U)		/*	�n�U�[�hSW=OFF									*/
#define TARGET_COM_DT_HZS_ON				((uint8) 1U)		/*	�n�U�[�hSW=ON									*/

/*	�P�ʐؑ֐M��2(KM/MILE)����ڕW�l(UNIT_CH2)	*/
#define TARGET_COM_DT_UNIT_NONE				((uint8) 0U)		/*	�P�ʕ\���Ȃ�									*/
#define TARGET_COM_DT_UNIT_KMH				((uint8) 1U)		/*	km/h											*/
#define TARGET_COM_DT_UNIT_MPH				((uint8) 2U)		/*	mph												*/

/*	���x����A����ڕW�l(TOLER_A)	*/
/*	��`����(���j�A�l�̈�)			*/

/*	���x����B����ڕW�l(TOLER_B)	*/
/*	��`����(���j�A�l�̈�)			*/

/*	����ݒ萧��ڕW�l(M_LANG)	*/
/*	��`����					*/

/*	����A�g��Ԑ���ڕW�l(M_LNG_ST)	*/
/*	��`����							*/

/*	�\���\����ꗗ1����ڕW�l(M_LNGDB1)	*/
/*	��`����								*/

/*	�\���\����ꗗ2����ڕW�l(M_LNGDB2)	*/
/*	��`����								*/

/*	�\���\����ꗗ3����ڕW�l(M_LNGDB3)	*/
/*	��`����								*/

/*	�\���\����ꗗ4����ڕW�l(M_LNGDB4)	*/
/*	��`����								*/

/*	�\���\����ꗗ5����ڕW�l(M_LNGDB5)	*/
/*	��`����								*/

/*	�\���\����ꗗ6����ڕW�l(M_LNGDB6)	*/
/*	��`����								*/

/*	�\���\����ꗗ7����ڕW�l(M_LNGDB7)	*/
/*	��`����								*/

/*	�ݒ�\�P�ʈꗗ�P����ڕW�l(M_UNTDB1)	*/
/*	��`����								*/

/*	LDA�x��^�C�~���O�X�e�A�����OSW(Enter)���쐧��ڕW�l(LDAMCUS)	*/
#define TARGET_COM_DT_LDAMCUS_UNDEF			((uint8) 0U)		/*	����`											*/
#define TARGET_COM_DT_LDAMCUS_HIGH			((uint8) 1U)		/*	High											*/
#define TARGET_COM_DT_LDAMCUS_STANDARD		((uint8) 2U)		/*	Standard										*/

/*	SWS���x�X�e�A�����OSW(Enter)���쐧��ڕW�l(FCMMCUS)	*/
#define TARGET_COM_DT_FCMMCUS_UNDEF			((uint8) 0U)		/*	����`											*/
#define TARGET_COM_DT_FCMMCUS_HIGH			((uint8) 1U)		/*	High											*/
#define TARGET_COM_DT_FCMMCUS_NORMAL		((uint8) 2U)		/*	Normal											*/
#define TARGET_COM_DT_FCMMCUS_LOW			((uint8) 3U)		/*	Low												*/

/*	SWS ON/OFF�X�e�A�����OSW(Enter)���쐧��ڕW�l(FCMMSW)	*/
#define TARGET_COM_DT_FCMMSW_OFF			((uint8) 0U)		/*	�\�t�gSW������									*/
#define TARGET_COM_DT_FCMMSW_ON				((uint8) 1U)		/*	�\�t�gSW����									*/

/*	RCTA ON/OFF�X�e�A�����OSW(Enter)���쐧��ڕW�l(RCTAMSW)	*/
#define TARGET_COM_DT_RCTAMSW_OFF			((uint8) 0U)		/*	�\�t�gSW������									*/
#define TARGET_COM_DT_RCTAMSW_ON			((uint8) 1U)		/*	�\�t�gSW����									*/

/*	BSM ON/OFF�X�e�A�����OSW(Enter)���쐧��ڕW�l(BSMMSW)	*/
#define TARGET_COM_DT_BSMMSW_OFF			((uint8) 0U)		/*	�\�t�gSW������									*/
#define TARGET_COM_DT_BSMMSW_ON				((uint8) 1U)		/*	�\�t�gSW����									*/

/*	PCS���x�ؑփX�e�A�����OSW(Enter)���쐧��ڕW�l(PCSMCUS)	*/
#define TARGET_COM_DT_PCSMCUS_OFF			((uint8) 0U)		/*	�\�t�gSW������									*/
#define TARGET_COM_DT_PCSMCUS_ON			((uint8) 1U)		/*	�\�t�gSW����									*/

/*	PCS ON/OFF�X�e�A�����OSW(Enter)���쐧��ڕW�l(PCSMSW)	*/
#define TARGET_COM_DT_PCSMSW_OFF			((uint8) 0U)		/*	�\�t�gSW������									*/
#define TARGET_COM_DT_PCSMSW_ON				((uint8) 1U)		/*	�\�t�gSW����									*/

/*	TBC�u���[�L�^�C�v�I���X�e�A�����OSW(Enter)���쐧��ڕW�l(TB_SLCT)	*/
#define TARGET_COM_DT_TB_SLCT_OFF			((uint8) 0U)		/*	�\�t�gSW������									*/
#define TARGET_COM_DT_TB_SLCT_ON			((uint8) 1U)		/*	�\�t�gSW����									*/

/*	�h���C�u���j�^�\���M��(�u�ԔR��)����ڕW�l(IN_FC)	*/
/*	��`����										*/

/*	�h���C�u���j�^�\���M��(�n���㕽�ώԑ�)����ڕW�l(AS_SP)	*/
/*	��`����											*/

/*	�h���C�u���j�^�\���M��(�n���㑖�s����)����ڕW�l(AS_DT)	*/
/*	��`����											*/

/*	�h���C�u���j�^�\���M��(�����㕽�ϔR��)����ڕW�l(AF_FC)	*/
/*	��`����											*/

/*	�h���C�u���j�^�\���M��(�q���\����)����ڕW�l(RANGE)	*/
/*	��`����											*/

/*	�h���C�u���j�^�\���M��(�ʎZ���ϔR��)����ڕW�l(TO_FC)	*/
/*	��`����											*/

/*	�h���C�u���j�^�\���M��(�n���㕽�ϔR��)����ڕW�l(AS_FC)	*/
/*	��`����											*/

/*	�h���C�u���j�^�\���M��(�n���㑖�s����)����ڕW�l(AS_TM)	*/
/*	��`����											*/

/*	�h���C�u���j�^�\���M��(�ʎZ���s����) �o�͒l(TO_TM)	*/
/*	��`����										*/

/*	���C���^���N�t���[�G���c�ʐM���l����ڕW�l(FUEL_1)	*/
#define TARGET_COM_DT_FUEL1_UNKNOWN			((uint16) 65535U)	/* ���m��											*/

/*	���C���^���N�t���[�G���c�ʒ�R�l����ڕW�l(FUEL_AD)	*/
#define TARGET_COM_DT_FUELAD_UNKNOWN		((uint16) 65535U)	/* ���m��											*/

/*	�I�h�l(�ʐM)����ڕW�l(ODO)	*/
#define	TARGET_COM_DT_ODTR_ODO_COM_INIT		((uint32) 0U)		/*	�I�h(�ʐM)�����l								*/
#define	TARGET_COM_DT_ODTR_ODO_COM_MAX		((uint32) 999999U)	/*	�I�h(�ʐM)�ő�l								*/

/*	�g���b�vA(�ʐM)����ڕW�l(TRIP_A)	*/
#define	TARGET_COM_DT_ODTR_TRIPA_COM_INIT	((uint32) 0U)		/*	�g���b�vA(�ʐM)�����l							*/
#define	TARGET_COM_DT_ODTR_TRIPA_COM_MAX	((uint32)99999U)	/*	�g���b�vA(�ʐM)�ő�l							*/

/*	�g���b�vB(�ʐM)����ڕW�l(TRIP_B)	*/
#define	TARGET_COM_DT_ODTR_TRIPB_COM_INIT	((uint32) 0U)		/*	�g���b�vB(�ʐM)�����l							*/
#define	TARGET_COM_DT_ODTR_TRIPB_COM_MAX	((uint32)99999U)	/*	�g���b�vB(�ʐM)�ő�l							*/

/*	Fuel���ϒl����ڕW�l	*/
#define	TARGET_COM_DT_FLAS_MIN				((uint8) 0U)		/*	Fuel���ϒl�M���ŏ��l							*/
#define	TARGET_COM_DT_FLAS_MAX				((uint8) 253U)		/*	Fuel���ϒl�M���ő�l							*/
#define	TARGET_COM_DT_FLAS_OPEN				((uint8) 254U)		/*	Fuel���ϒl�M���Z���_�I�[�v��					*/
#define	TARGET_COM_DT_FLAS_UNKNOWN			((uint8) 255U)		/*	Fuel���ϒl�M���f�[�^���m��						*/
/* �����[�U�[�ҏW�\�� */

#endif	/* BSW_COMMON_COMDEF_INC */

