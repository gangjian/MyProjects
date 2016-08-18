/*	$RCSfile: Rte_swc_in_oilp.c $										*/
/*	$Date: 2015/11/26 20:56:41JST $										*/
/*	$Revision: 1.2 $													*/
/*	 EXPLANATION: �I�C���v���b�V���Q�[�W(�w�j�\��)(����) �\�[�X�t�@�C��	*/

/**
 * \file Rte_swc_in_oilp_template.c
 *
 * \brief <Add a description here>
 *
 * \b Generator: Picea Rte V4.9.0-Delivery-Build275
 */

/*============================================================================*
 * PREPROCESSOR DIRECTIVES                                                    *
 *============================================================================*/

/* INCLUDE DIRECTIVES FOR OTHER HEADERS --------------------------------------*/

#include "Rte_swc_in_oilp.h"

/*============================================================================*
 * PREPROCESSOR DIRECTIVES                                                    *
 *============================================================================*/

/* INCLUDE DIRECTIVES FOR OTHER HEADERS --------------------------------------*/

#include "bsw_common.h"							/* BSW���ʃw�b�_�t�@�C��														*/
#include "Rte_swc_in_oilp_map.h"				/* �I�C���v���b�V���Q�[�W(�w�j�\��)(����) �R���t�B�M�����[�V�����w�b�_�t�@�C��	*/
/* EXPORTED DEFINES FOR CONSTANTS --------------------------------------------*/

/*-----------------------------------------------------------------------------/
 *
 * Used Enumeration Data types
 * - cmp_common_inMsgSts (uint8)
 *   - IN_MSGSTS_NONE
 *   - IN_MSGSTS_NG
 *   - IN_MSGSTS_FAILINIT
 *   - IN_MSGSTS_TXSTOP
 *   - IN_MSGSTS_TIMEOUT
 *   - IN_MSGSTS_NORX
 * - cmp_common_pvSts (uint8)
 *   - PV_STS_NORMAL
 *   - PV_STS_SHORT
 *   - PV_STS_TIMEOUT
 *   - PV_STS_RNGOVR
 *   - PV_STS_INVALID
 *   - PV_STS_JUDGING
 *   - PV_STS_FAIL
 *   - PV_STS_NOTRCV
 *   - PV_STS_ERR
 *   - PV_STS_UNKNOWN
 * - cmp_common_adSts (uint8)
 *   - AD_STS_NORMAL
 *   - AD_STS_ABNORMAL
 *   - AD_STS_JUDGING
 *   - AD_STS_UNKNOWN
 * - cmp_in_NE1 (sint16)
 *   - IN_NE1_MIN
 *   - IN_NE1_MAX
 * - cmp_ioHwAb_normalAdcCh (uint32)
 *   - IN_NORMALADC_CH_0
 *   - IN_NORMALADC_CH_1
 *   - IN_NORMALADC_CH_2
 *   - IN_NORMALADC_CH_3
 *   - IN_NORMALADC_CH_MAX
 * - cmp_ioHwAb_normalAdcVal (uint16)
 *   - IN_NORMALADC_VAL_MIN
 *   - IN_NORMALADC_VAL_MAX
 * - cmp_pv_PvEngOnOff3s (uint8)
 *   - PV_ENGONOFF3S_OFF
 *   - PV_ENGONOFF3S_ON
 * - cmp_pv_PvOilpAd (uint16)
 *   - PV_OILPAD_MIN
 *   - PV_OILPAD_MAX
 * - cmp_timer_id (uint16)
 *   - TIMID_PRC_OILP_OILPAD
 *   - TIMID_IN_OILP_IGAD
 *   - TIMID_NUM
 * - cmp_timer_kind (uint8)
 *   - TM_TIMTYP_ONESHOT
 *   - TM_TIMTYP_CYCLIC
 *   - TM_TIMTYP_SYNC_ONE
 *   - TM_TIMTYP_SYNC_CYC
 * - cmp_timer_sts (uint8)
 *   - TM_TIMSTS_STOP
 *   - TM_TIMSTS_START
 *   - TM_TIMSTS_TIMEOUT
 *
 *----------------------------------------------------------------------------*/

/* �G���W����]�� */
#define NE1SIGNED_IN_DATA_MIN			((uint16) 0x0000)					/* �G���W����]��signed�̍ŏ��l (LSB:0.78125(rpm))	*/

#define ENGREV3S_IN_MIN					((uint16) 0x0000)					/* �G���W����]���̍ŏ��l (LSB:0.78125(rpm))		*/
#define ENGREV3S_IN_MAX					((uint16) 0x4000)					/* �G���W����]���̍ő�l (LSB:0.78125(rpm))		*/
#define ENGREV3S_IN_STS_NORMAL			((uint8) 0x00)						/* ����												*/
#define ENGREV3S_IN_STS_FAIL			((uint8) 0x01)						/* �t�F�[����										*/
#define ENGREV3S_IN_STS_UNFIX			((uint8) 0x02)						/* ���m��											*/

/* IG�d��A/D�l */
#define IGV_IN_IG_AD_MIN				((uint16)0)							/* IG�d��A/D�l�F�ŏ��l								*/

/* IG�d��A/D�l�f�[�^��� */
#define IGV_IN_AD_ST_INACTIVE			((uint8) 0x00)						/* ���m��											*/
#define IGV_IN_AD_ST_NORMAL				((uint8) 0x01)						/* �m��												*/
#define IGV_IN_AD_ST_UNUSUAL			((uint8) 0x02)						/* �ُ�												*/

/* �d���␳ */
#define OILPAD_CORRECT_COEFFICIENT		((uint32) 65536)					/* ���x����␳�W��									*/

/* �T���v�����O�� */
#define OILPAD_SAMPSTS_NG				((uint8) 0x00)						/* �T���v�����O��									*/
#define OILPAD_SAMPSTS_OK				((uint8) 0x01)						/* �T���v�����O��									*/

/* IG�d������AD�l */
#define OILPAD_IGAD_INIT				((uint16) 0x0000)					/* IG�d������AD�l:�����l							*/
#define OILPAD_IGAD_STS_UNKNOWN			((uint8)  0x00)						/* IG�d��AD�l���:���m����						*/
#define OILPAD_IGAD_STS_SIMPLE_AVRG		((uint8)  0x01)						/* IG�d��AD�l���:�P�����Ϗ��						*/
#define OILPAD_IGAD_STS_MOVE_AVRG		((uint8)  0x02)						/* IG�d��AD�l���:�ړ����Ϗ��						*/
#define OILPAD_IGAD_AVRG_CNT			((uint8)  4)						/* IG�d�����ω���									*/
#define OILPAD_IGAD_AVRG_INIT_CNT		((uint8)  0)						/* IG�d���~�σJ�E���^�����l							*/


#define OILP_UINT16_MAX					((uint16) 0xFFFF)					/* uint16�^�ő�l									*/

/* AD�l�V�t�g */
#define OILPAD_AD_SHIFT_BIT				((uint16) 0x0002)					/* �V�t�g�r�b�g��									*/
#define OILPAD_AD_MAX					((uint16) 0x03FF)					/* AD�ő�l											*/

/*============================================================================*
 * EXPORTED TYPEDEF DECLARATIONS                                              *
 *============================================================================*/

/*-----------------------------------------------------------------------------/
 *
 * Used Implementation Data Types
 *  uint8
 *  uint16
 *  uint8
 *  sint16
 *  uint32
 *  uint16
 *  pvU1: Record with 2 fields.
 *  pvU2: Record with 2 fields.
 *
 *----------------------------------------------------------------------------*/

/*============================================================================*
 * EXPORTED OBJECT DECLARATIONS                                               *
 *============================================================================*/
/* Port interfaces -----------------------------------------------------------*/

/* Per-Instance-Memorys ------------------------------------------------------*/

/* Calibration Datas ----------------------------------------------------------*/

/* Calibration Parameters ----------------------------------------------------*/


/*============================================================================*
 * GLOBAL PARAMETERS		                                                  *
 *============================================================================*/


/*============================================================================*
 * STATIC PARAMETERS		                                                  *
 *============================================================================*/
#define swc_in_oilp_START_SEC_VAR_INIT_8
#include "swc_in_oilp_MemMap.h"

static uint8 igvAdSts;															/* IG�d��A/D�l�f�[�^���										*/
static uint8 oilpAdIgAdAvrgSts;													/* IG�d������AD�l���											*/
static uint8 oilpAdIgAdAccumuCnt;												/* IG�d������AD�ݐσJ�E���^										*/

#define swc_in_oilp_STOP_SEC_VAR_INIT_8
#include "swc_in_oilp_MemMap.h"


#define swc_in_oilp_START_SEC_VAR_INIT_16
#include "swc_in_oilp_MemMap.h"

static uint16 igvAd;															/* IG�d��A/D�l													*/
static uint16 oilpAdPreIgAdAvrg;												/* �O��IG�d������AD�l											*/

#define swc_in_oilp_STOP_SEC_VAR_INIT_16
#include "swc_in_oilp_MemMap.h"


#define swc_in_oilp_START_SEC_VAR_INIT_UNSPECIFIED
#include "swc_in_oilp_MemMap.h"

static uint16 oilpAdIgAdAvrgBuf[OILPAD_IGAD_AVRG_CNT];							/* IG�d������AD�l�o�b�t�@										*/
static pvU1	  pvEngOnOff3s;														/* �d�m�f�@�n�m�^�n�e�e����(�t�F�[������3s)�̕����l				*/

#define swc_in_oilp_STOP_SEC_VAR_INIT_UNSPECIFIED
#include "swc_in_oilp_MemMap.h"


/*============================================================================*
 * STATIC FUNCTIONS PROTOTYPES                                                *
 *============================================================================*/
static void initPvOilp(void);													/* ����������											*/
static uint16 makeNe1Signed(void);												/* �G���W����]��signed��������							*/
static void makeEngRevFail3s(uint16 *engRev3s, uint8 *engRevSts3s);				/* �G���W����]��(�t�F�[������3s)��������				*/
static void makeEngOnOff3s(void);												/* �d�m�f�@�n�m�^�n�e�e����(�t�F�[������3s)��������		*/
static void makeIgv(void);														/* IG�d�����͏�񐶐�����								*/
static void makeOilpAd(void);													/* �I�C���v���b�V���Z���_�M����񐶐�����				*/
static uint8 jdgSampSts(void);													/* �T���v�����O�J�n�۔��菈��							*/
static uint16 calcIgAdAvrgData(uint8 sampSts);									/* IG�d������AD�l�Z�o����								*/
static uint16 correctOilpAd(uint16 igAdAvrg, uint16 oilpAd);					/* �Z���_�d���␳����									*/


/*============================================================================*
 * EXPORTED FUNCTIONS PROTOTYPES                                              *
 *============================================================================*/
/* Declaration of runnable entities ------------------------------------------*/

/*-----------------------------------------------------------------------------/
 *
 * Runnable Entity: sym_rbl_in_oilp
 *  Can be invoked concurrently: TRUE
 *  Events:
 *  - Triggerred by ev_te_in_oilp (TIMING-EVENT) once every 0.01 seconds
 *
 *-----------------------------------------------------------------------------/
 *
 * Implicit Sender-Reciever Data-Read-Access API(s): 
 * - uint16 Rte_IRead_rbl_in_oilp_rp_srIf_common_igOnTime_val(void)
 * - sint16 Rte_IRead_rbl_in_oilp_rp_srIf_in_NE1_val(void)
 * - uint8 Rte_IRead_rbl_in_oilp_rp_srIf_in_NE1Sts_val(void)
 *
 * Implicit Sender-Reciever Data-Write-Access API(s): 
 * - void Rte_IWrite_rbl_in_oilp_pp_srIf_pv_PvEngOnOff3s_struct(const pvU1* struct)
 * - pvU1* Rte_IWriteRef_rbl_in_oilp_pp_srIf_pv_PvEngOnOff3s_struct(void)
 * - void Rte_IWrite_rbl_in_oilp_pp_srIf_pv_PvOilpAd_struct(const pvU2* struct)
 * - pvU2* Rte_IWriteRef_rbl_in_oilp_pp_srIf_pv_PvOilpAd_struct(void)
 *
 * Synchronous Client-Server API(s): 
 * - Std_ReturnType Rte_Call_rp_csIf_timer_startTimer_op(uint16 id, uint8 type, uint16 period)
 * - Std_ReturnType Rte_Call_rp_csIf_timer_sts_op(uint16 id, uint8* sts)
 * - Std_ReturnType Rte_Call_rp_csIf_ioHwAb_normalAdcStsGet_op(uint32 id, uint8* sts)
 * - Std_ReturnType Rte_Call_rp_csIf_ioHwAb_normalAdcValGet_op(uint32 id, uint16* val)
 *
 * Mode API(s): 
 * - uint8 Rte_Mode_rp_msIf_ig_ModeDeclGroup_ig(void)
 *
 *----------------------------------------------------------------------------*/

#define swc_in_oilp_START_SEC_CODE
#include "swc_in_oilp_MemMap.h"

/********************************************************************************/
/*	�֐��T�v�F�I�C���v���b�V���Q�[�W(�w�j�\��)(����) �^�X�N����					*/
/*	���������F																	*/
/*																				*/
/*	����	�F����																*/
/*	�߂�l	�F����																*/
/*	���ӎ����F����																*/
/********************************************************************************/
FUNC(void, swc_in_oilp_CODE) sym_rbl_in_oilp(void)
{
	makeEngOnOff3s();														/* �d�m�f�@�n�m�^�n�e�e����(�t�F�[������3s)��������	*/
	makeIgv();																/* IG�d�����͏�񐶐�����							*/
	makeOilpAd();															/* �I�C���v���b�V���Z���_�M����񐶐�����			*/
}

#define swc_in_oilp_STOP_SEC_CODE
#include "swc_in_oilp_MemMap.h"


#define swc_in_oilp_START_SEC_CODE
#include "swc_in_oilp_MemMap.h"

/********************************************************************************/
/*	�֐��T�v�F�I�C���v���b�V���Q�[�W(�w�j�\��)(����) �����������i���Z�b�g�j		*/
/*	���������F																	*/
/*																				*/
/*	����	�F����																*/
/*	�߂�l	�F����																*/
/*	���ӎ����F����																*/
/********************************************************************************/
FUNC(void, swc_in_oilp_CODE) sym_rbl_in_oilp_initReset(void)
{
	pvU2 oilpAd;																/* OIL-P�d��A/D�l�����l								*/

	initPvOilp();

	oilpAd.dt = PV_OILPAD_MIN;													/* �����l��RTE�֏o��								*/
	oilpAd.sts = PV_STS_UNKNOWN;

	Rte_IWrite_rbl_in_oilp_initReset_pp_srIf_pv_PvEngOnOff3s_struct(&pvEngOnOff3s);
	Rte_IWrite_rbl_in_oilp_initReset_pp_srIf_pv_PvOilpAd_struct(&oilpAd);
}

#define swc_in_oilp_STOP_SEC_CODE
#include "swc_in_oilp_MemMap.h"


#define swc_in_oilp_START_SEC_CODE
#include "swc_in_oilp_MemMap.h"

/********************************************************************************/
/*	�֐��T�v�F�I�C���v���b�V���Q�[�W(�w�j�\��)(����) ����������					*/
/*			  �i�E�F�C�N�A�b�v�j												*/
/*	���������F																	*/
/*																				*/
/*	����	�F����																*/
/*	�߂�l	�F����																*/
/*	���ӎ����F����																*/
/********************************************************************************/
FUNC(void, swc_in_oilp_CODE) sym_rbl_in_oilp_initWakeup(void)
{
	pvU2 oilpAd;																/* OIL-P�d��A/D�l�����l								*/

	initPvOilp();

	oilpAd.dt = PV_OILPAD_MIN;													/* �����l��RTE�֏o��								*/
	oilpAd.sts = PV_STS_UNKNOWN;
	Rte_IWrite_rbl_in_oilp_initWakeup_pp_srIf_pv_PvEngOnOff3s_struct(&pvEngOnOff3s);
	Rte_IWrite_rbl_in_oilp_initWakeup_pp_srIf_pv_PvOilpAd_struct(&oilpAd);
}

#define swc_in_oilp_STOP_SEC_CODE
#include "swc_in_oilp_MemMap.h"


#define swc_in_oilp_START_SEC_CODE
#include "swc_in_oilp_MemMap.h"

/********************************************************************************/
/*	�֐��T�v�F����������														*/
/*	���������F																	*/
/*																				*/
/*	����	�F����																*/
/*	�߂�l	�F����																*/
/*	���ӎ����F����																*/
/********************************************************************************/
static void initPvOilp(void)
{
	uint8 cnt;								/* ���[�v�J�E���^						*/

	pvEngOnOff3s.dt = PV_ENGONOFF3S_OFF;
	pvEngOnOff3s.sts = PV_STS_NOTRCV;

	igvAd = IGV_IN_IG_AD_MIN;
	igvAdSts = IGV_IN_AD_ST_INACTIVE;

	oilpAdIgAdAvrgSts = OILPAD_IGAD_STS_UNKNOWN;								/* 3	: IG�d������AD�l��Ԃ�IG�d��AD�l���:���m���Ԃ�ݒ�			*/
	oilpAdIgAdAccumuCnt = OILPAD_IGAD_AVRG_INIT_CNT;							/* 4	: IG�d������AD�ݐσJ�E���^��IG�d���~�σJ�E���^�����l��ݒ�		*/
	oilpAdPreIgAdAvrg = OILPAD_IGAD_INIT;										/* 5	: �O��IG�d������AD�l��IG�d������AD�l:�����l��ݒ�				*/

	for(cnt=0; cnt<OILPAD_IGAD_AVRG_CNT; cnt++){								/* 6	: IG�d�����ω��񐔕��C�ȉ��̏��������{							*/
		oilpAdIgAdAvrgBuf[cnt] = OILPAD_IGAD_INIT;								/* 		: IG�d������AD�l�o�b�t�@��IG�d������AD�l:�����l��ݒ�			*/
	}
}

#define swc_in_oilp_STOP_SEC_CODE
#include "swc_in_oilp_MemMap.h"


#define swc_in_oilp_START_SEC_CODE
#include "swc_in_oilp_MemMap.h"

/********************************************************************************/
/*	�֐��T�v�F�G���W����]��signed��������										*/
/*	���������F																	*/
/*																				*/
/*	����	�F����																*/
/*	�߂�l	�F�G���W����]��(LSB:0.78125(rpm))									*/
/*	���ӎ����F����																*/
/********************************************************************************/
static uint16 makeNe1Signed(void)
{
	uint16 ne1;										/*	�G���W����]��				*/
	sint16 inNe1Signed;								/*	signed�^�̃G���W����]��	*/

	inNe1Signed = Rte_IRead_rbl_in_oilp_rp_srIf_in_NE1_val();
													/*	1		�F�G���W����]��signed�M���擾���������{									*/
													/*			�Fsigned�^�̃G���W����]���Ɂu�G���W����]��signed�M���v��ݒ�				*/
													/*	1-1		�F�G���W����]���͈͔̔���													*/
	if(inNe1Signed < IN_NE1_MIN){					/*	1-1-1	�Fsigned�^�̃G���W����]����signed�^�̃G���W����]���ŏ��l��菬�����ꍇ	*/
		ne1 = NE1SIGNED_IN_DATA_MIN;				/*			�F�G���W����]���Ɂu�G���W����]��signed�̍ŏ��l�v��ݒ�					*/

	}else{											/*	1-1-2	�F��L�ȊO�̏ꍇ															*/
		ne1 = (uint16)inNe1Signed;					/*			�F�G���W����]����signed�^�̃G���W����]����ݒ�							*/
	}
	return ne1;
}

#define swc_in_oilp_STOP_SEC_CODE
#include "swc_in_oilp_MemMap.h"


#define swc_in_oilp_START_SEC_CODE
#include "swc_in_oilp_MemMap.h"

/********************************************************************************/
/*	�֐��T�v�F�G���W����]��(�t�F�[������3s)��������							*/
/*	���������F																	*/
/*																				*/
/*	����	�F�������F�G���W����]��											*/
/*			�F��j�����F�G���W����]���f�[�^���								*/
/*	�߂�l	�F����																*/
/*	���ӎ����F����																*/
/********************************************************************************/
static void makeEngRevFail3s(uint16 *engRev3s, uint8 *engRevSts3s)
{
	uint8 igSts;										/*	IG���							*/
	uint8 ne1Sts;										/*	�G���W����]���f�[�^��Ԏ擾�l	*/

	igSts = Rte_Mode_rp_msIf_ig_ModeDeclGroup_ig();		/*	1				�FIG�����擾���AIG-ON/OFF����							*/

	if(igSts == RTE_MODE_IG7V_ON){						/*	1-1				�FIG-ON���̏ꍇ											*/

		ne1Sts = Rte_IRead_rbl_in_oilp_rp_srIf_in_NE1Sts_val();
														/*	1-1-1			�F�G���W����]���r����(3s)���擾���A�r�┻��			*/

		if((ne1Sts & IN_MSGSTS_FAILINIT) == IN_MSGSTS_NONE){
														/*	1-1-1-1			�F�r�▢�����̏ꍇ										*/

			if((ne1Sts & IN_MSGSTS_NORX) == IN_MSGSTS_NONE){
														/*	1-1-1-1-1-1		�F��M�̏ꍇ											*/

				*engRev3s = makeNe1Signed();			/*	1-1-1-1-1-1-1	�F�G���W����]��signed�����������R�[��					*/
														/*	1-1-1-1-1-1-2	�F�f�[�^�͈̔̓`�F�b�N									*/
				if(*engRev3s > ENGREV3S_IN_MAX){		/*	1-1-1-1-1-1-2-1	�F�G���W����]�����G���W����]���̍ő�l���傫���ꍇ	*/
					*engRev3s = ENGREV3S_IN_MAX;		/*					�F�G���W����]���ɃG���W����]���̍ő�l��ݒ�			*/
				}else{									/*	1-1-1-1-1-1-2-2	�F��L�ȊO�̏ꍇ										*/
					;									/*					�F��������												*/
				}
				*engRevSts3s = ENGREV3S_IN_STS_NORMAL;	/*	1-1-1-1-1-1-3	�F�G���W����]���f�[�^��ԂɁA�u����v��ݒ�			*/
			}else{										/*	1-1-1-1-1-2		�F��M�ȊO�̏ꍇ										*/

				*engRev3s = ENGREV3S_IN_MIN;			/*	1-1-1-1-1-2-1	�F�G���W����]���ɁA�u�G���W����]���̍ŏ��l�v��ݒ�	*/
				*engRevSts3s = ENGREV3S_IN_STS_UNFIX;	/*	1-1-1-1-1-2-2	�F�G���W����]���f�[�^��ԂɁA�u���m��v��ݒ�			*/
			}
		}else{											/*	1-1-1-2			�F�r�▢�����ȊO�̏ꍇ									*/
			*engRev3s = ENGREV3S_IN_MIN;				/*					�F�G���W����]���ɁA�u�G���W����]���̍ŏ��l�v��ݒ�	*/
			*engRevSts3s = ENGREV3S_IN_STS_FAIL;		/*					�F�G���W����]���f�[�^��ԂɁA�u�t�F�[�����v��ݒ�		*/
		}
	}else{												/*	1-2				�FIG-ON�ȊO�̏ꍇ										*/
		*engRev3s = ENGREV3S_IN_MIN;					/*					�F�G���W����]���ɁA�u�G���W����]���̍ŏ��l�v��ݒ�	*/
		*engRevSts3s = ENGREV3S_IN_STS_NORMAL;			/*					�F�G���W����]���f�[�^��ԂɁA�u����v��ݒ�			*/
	}
}

#define swc_in_oilp_STOP_SEC_CODE
#include "swc_in_oilp_MemMap.h"


#define swc_in_oilp_START_SEC_CODE
#include "swc_in_oilp_MemMap.h"

/********************************************************************************/
/*	�֐��T�v�F�d�m�f�@�n�m�^�n�e�e����(�t�F�[������3s)��������					*/
/*	���������F																	*/
/*																				*/
/*	����	�F����																*/
/*	�߂�l	�F����																*/
/*	���ӎ����F����																*/
/********************************************************************************/
static void makeEngOnOff3s(void)
{
	uint16 inputNe1;													/* �G���W����]��										*/
	uint8  igSts;														/* IG���												*/
	uint8  ne1Sts;														/* �G���W����]���f�[�^���								*/
	uint8  usedInfo;													/* �M���L�����											*/

	usedInfo = ENGONOFF3S_GET_CM_NE1();									/* 1,2				: �G���W����]���ݒ���擾								*/
	if( usedInfo == ENGONOFF3S_CM_NE1_USED ) {							/* 1				: �G���W����]���ݒ肪�g�p �̏ꍇ						*/
		igSts = Rte_Mode_rp_msIf_ig_ModeDeclGroup_ig();					/* 1-1,1-2			: IG�����擾											*/
		if( igSts == RTE_MODE_IG7V_ON ) { 								/* 1-1				: IG-ON�̏ꍇ											*/
			makeEngRevFail3s(&inputNe1, &ne1Sts);						/*					: �G���W����]���f�[�^���(�t�F�[������3s)�A			*/
																		/*					: �G���W����]��(�t�F�[������3s)�𐶐�					*/
			if( ne1Sts == ENGREV3S_IN_STS_NORMAL ) {					/* 1-1-1			: ����̏ꍇ											*/
				if( inputNe1 >= ENGONOFF3S_IN_NE1_ON_LIMIT ) {			/* 1-1-1-1			: �G���W���n�m����臒l�ȏ�̏ꍇ						*/
					pvEngOnOff3s.dt = PV_ENGONOFF3S_ON;					/*					: �d�m�f�@�n�m�^�n�e�e����̕����l���uON�v				*/
				} else if( inputNe1 < ENGONOFF3S_IN_NE1_OFF_LIMIT ) {	/* 1-1-1-2			: �G���W���n�e�e����臒l��菬�����ꍇ					*/
					pvEngOnOff3s.dt = PV_ENGONOFF3S_OFF; 				/*					: �d�m�f�@�n�m�^�n�e�e����̕����l���uOFF�v				*/
				} else {												/* 1-1-1-3			: ��L�ȊO�̏ꍇ										*/
					;
				}
				pvEngOnOff3s.sts = PV_STS_NORMAL;						/* 1-1-1-4			: �d�m�f�@�n�m�^�n�e�e����f�[�^��Ԃ̕����l���u����v	*/
			} else if( ne1Sts == ENGREV3S_IN_STS_FAIL ) {				/* 1-1-2			: �t�F�[�����̏ꍇ										*/
				pvEngOnOff3s.dt = PV_ENGONOFF3S_OFF; 					/*					: �d�m�f�@�n�m�^�n�e�e����̕����l���uOFF�v 			*/
				pvEngOnOff3s.sts = PV_STS_FAIL;							/*					: �d�m�f�@�n�m�^�n�e�e����f�[�^��Ԃ̕����l���u̪�ْ��v*/
			} else {													/* 1-1-3			: ��L�ȊO�̏ꍇ										*/
				pvEngOnOff3s.dt = PV_ENGONOFF3S_OFF;					/*					: �d�m�f�@�n�m�^�n�e�e����̕����l���uOFF�v 			*/
				pvEngOnOff3s.sts = PV_STS_NOTRCV;						/*					: �d�m�f�@�n�m�^�n�e�e����f�[�^��Ԃ̕����l���u���m��v*/
			}
		} else {														/* 1-2				: IG-ON�ȊO�̏ꍇ										*/
			pvEngOnOff3s.dt = PV_ENGONOFF3S_OFF;						/*					: �d�m�f�@�n�m�^�n�e�e����̕����l���uOFF�v 			*/
			pvEngOnOff3s.sts = PV_STS_NOTRCV;							/*					: �d�m�f�@�n�m�^�n�e�e����f�[�^��Ԃ̕����l���u���m��v*/
		}
	} else {															/* 2				: �G���W����]���ݒ肪�g�p �ȊO�̏ꍇ					*/
		pvEngOnOff3s.dt = PV_ENGONOFF3S_OFF;							/*					: �d�m�f�@�n�m�^�n�e�e����̕����l���uOFF�v 			*/
		pvEngOnOff3s.sts = PV_STS_NOTRCV;								/*					: �d�m�f�@�n�m�^�n�e�e����f�[�^��Ԃ̕����l���u���m��v*/
	}
																		/*					: �����l��RTE�֏o��										*/
	Rte_IWrite_rbl_in_oilp_pp_srIf_pv_PvEngOnOff3s_struct(&pvEngOnOff3s);
}

#define swc_in_oilp_STOP_SEC_CODE
#include "swc_in_oilp_MemMap.h"


#define swc_in_oilp_START_SEC_CODE
#include "swc_in_oilp_MemMap.h"

/********************************************************************************/
/*	�֐��T�v�FIG�d�����͏�񐶐�����											*/
/*	���������F																	*/
/*																				*/
/*	����	�F����																*/
/*	�߂�l	�F����																*/
/*	���ӎ����F����																*/
/********************************************************************************/
static void makeIgv(void)
{
	uint16			 igvTimeSts;							/*	IG-ON��o�ߎ���				*/
	uint8			 adSts;									/*	A/D�ϊ����					*/
	uint16			 igvAdInfor;							/*	IG�d��A/D�l					*/
	Std_ReturnType	 csIfGetRet;							/*  csIf(get)�߂�l				*/

	csIfGetRet = RTE_E_OK;
	igvTimeSts = Rte_IRead_rbl_in_oilp_rp_srIf_common_igOnTime_val();
															/*	1		�FIG-ON��o�ߎ��Ԏ擾���������{���CIG-ON��o�ߎ��Ԃ��擾����B			*/
	if(igvTimeSts >= IGV_STABILITY_PERIOD){					/*	2		�FIG-ON��o�ߎ���(�߂�l)��IG�d�����艻�҂����Ԃ̏ꍇ					*/
		csIfGetRet = Rte_Call_rp_csIf_ioHwAb_normalAdcStsGet_op(IN_NORMALADC_CH_3, &adSts);
															/*			�FA/D�ϊ���Ԏ擾���������{���CA/D�ϊ���Ԃ��擾����B					*/
		if(csIfGetRet == RTE_E_OK){
			if(adSts == AD_STS_NORMAL){						/*	2-1		�FA/D�ϊ����(�߂�l)���y�m��z�̏ꍇ									*/
				csIfGetRet = Rte_Call_rp_csIf_ioHwAb_normalAdcValGet_op(IN_NORMALADC_CH_3, &igvAdInfor);
															/*			�FIG�d��A/D�l�擾���������{���CIG�d��A/D�l���擾����B					*/
				if(csIfGetRet == RTE_E_OK){
					igvAdInfor = igvAdInfor >> OILPAD_AD_SHIFT_BIT;
					if(OILPAD_AD_MAX < igvAdInfor){			/*	2-1-1	�FIG�d��A/D�l���͈͊O�̏ꍇ												*/
						igvAd = IGV_IN_IG_AD_MIN;			/*			�FIG�d��A/D�l�ɁuIG�d��A/D�l�F�ŏ��l�v��ݒ肷��B						*/
						igvAdSts = IGV_IN_AD_ST_UNUSUAL;	/*			�FIG�d��A/D�l�f�[�^��ԂɁu�ُ�v��ݒ肷��B							*/
					}else{									/*	2-1-2	�FIG�d��A/D�l(�߂�l)����L�ȊO�̏ꍇ									*/
						igvAd = igvAdInfor;					/*			�FIG�d��A/D�l�ɁuIG�d��A/D�l(�߂�l)�v��ݒ肷��B						*/
						igvAdSts = IGV_IN_AD_ST_NORMAL;		/*			�FIG�d��A/D�l�f�[�^��ԂɁu�m��v��ݒ肷��B							*/
					}
				} else {
					igvAd = IGV_IN_IG_AD_MIN;
					igvAdSts = IGV_IN_AD_ST_INACTIVE;
				}
			}else if(adSts == AD_STS_ABNORMAL){				/*	2-2		�FA/D�ϊ����(�߂�l)���y�ُ�z�̏ꍇ									*/
				igvAd = IGV_IN_IG_AD_MIN;					/*			�FIG�d��A/D�l�ɁuIG�d��A/D�l�F�ŏ��l�v��ݒ肷��B						*/
				igvAdSts = IGV_IN_AD_ST_UNUSUAL;			/*			�FIG�d��A/D�l�f�[�^��ԂɁu�ُ�v��ݒ肷��B							*/
			}else if(adSts == AD_STS_UNKNOWN){				/*	2-3		�FA/D�ϊ����(�߂�l)���y���m��z�̏ꍇ									*/
				igvAd = IGV_IN_IG_AD_MIN;					/*			�FIG�d��A/D�l�ɁuIG�d��A/D�l�F�ŏ��l�v��ݒ肷��B						*/
				igvAdSts = IGV_IN_AD_ST_INACTIVE;			/*			�FIG�d��A/D�l�f�[�^��ԂɁu���m��v��ݒ肷��B							*/
			}else{											/*	2-4		�FA/D�ϊ����(�߂�l)����L�ȊO�̏ꍇ									*/
				igvAd = IGV_IN_IG_AD_MIN;					/*			�FIG�d��A/D�l�ɁuIG�d��A/D�l�F�ŏ��l�v��ݒ肷��B						*/
				igvAdSts = IGV_IN_AD_ST_INACTIVE;			/*			�FIG�d��A/D�l�f�[�^��ԂɁu���m��v��ݒ肷��B							*/
			}
		} else {
			igvAd = IGV_IN_IG_AD_MIN;
			igvAdSts = IGV_IN_AD_ST_INACTIVE;
		}
	}else{													/*	3		�FIG-ON��o�ߎ���(�߂�l)����L�ȊO�̏ꍇ								*/
		igvAd = IGV_IN_IG_AD_MIN;							/*			�FIG�d��A/D�l�ɁuIG�d��A/D�l�F�ŏ��l�v��ݒ肷��B						*/
		igvAdSts = IGV_IN_AD_ST_INACTIVE;					/*			�FIG�d��A/D�l�f�[�^��ԂɁu���m��v��ݒ肷��B							*/
	}
}

#define swc_in_oilp_STOP_SEC_CODE
#include "swc_in_oilp_MemMap.h"


#define swc_in_oilp_START_SEC_CODE
#include "swc_in_oilp_MemMap.h"

/********************************************************************************/
/*	�֐��T�v�F�I�C���v���b�V���Z���_�M����񐶐�����							*/
/*	���������F																	*/
/*																				*/
/*	����	�F����																*/
/*	�߂�l	�F����																*/
/*	���ӎ����F����																*/
/********************************************************************************/
static void makeOilpAd(void)
{
	uint8			 sampSts;							/* �T���v�����O�J�n�۔���				*/
	uint16			 igAdAvrg;							/* IG�d������AD�l						*/
	uint8			 adSts;								/* A/D�ϊ����							*/
	uint16			 correctResult;						/* �␳�I�C���v���b�V���Z���_�d��AD�l	*/
	pvU2			 oilpAd;							/* OIL-P�d��A/D�l�o�͒l					*/
	uint16			 inputH2k;							/* �I�C���v���b�V���Z���_�d��			*/
	Std_ReturnType	 csIfGetRet;						/*  csIf(get)�߂�l						*/

	csIfGetRet = RTE_E_OK;
	sampSts = jdgSampSts();															/* 1		: �T���v�����O�J�n�۔�����擾							*/
	igAdAvrg = calcIgAdAvrgData(sampSts);											/* 2		: IG�d������AD�l���擾										*/

	if(sampSts == OILPAD_SAMPSTS_OK){												/* 3		: �T���v�����O�J�n�۔��肪�y�T���v�����O�z�̏ꍇ		*/
		csIfGetRet = Rte_Call_rp_csIf_ioHwAb_normalAdcStsGet_op(IN_NORMALADC_CH_0, &adSts);	/* 		: �I�C���v���b�V���Z���_�d����A/D�ϊ���Ԃ��擾			*/
		if(csIfGetRet == RTE_E_OK){
			switch(adSts){
			case AD_STS_NORMAL:														/* 3-1		: A/D�ϊ���Ԃ��y�m��z�̏ꍇ								*/
				csIfGetRet = Rte_Call_rp_csIf_ioHwAb_normalAdcValGet_op(IN_NORMALADC_CH_0, &inputH2k);
				inputH2k = inputH2k >> OILPAD_AD_SHIFT_BIT;
				if(csIfGetRet == RTE_E_OK){
					correctResult = correctOilpAd(igAdAvrg, inputH2k);				/* 			: �␳�I�C���v���b�V���Z���_�d��AD�l���擾					*/
					if(correctResult >= PV_OILPAD_MAX){								/* 3-1-1	: �␳�I�C���v���b�V���Z���_�d��AD�l��						*/
																					/* 								�yOIL-P�d��A/D�l:�ő�l�z�ȏ�̏ꍇ		*/
						oilpAd.dt = PV_OILPAD_MAX;									/* 			: OIL-P�d��A/D�l��OIL-P�d��A/D�l:�ő�l��ݒ�				*/
					}else{															/* 3-1-2	: �␳�I�C���v���b�V���Z���_�d��AD�l����L�ȊO�̏ꍇ		*/
						oilpAd.dt = correctResult;									/* 			: OIL-P�d��A/D�l�ɕ␳�I�C���v���b�V���Z���_�d��AD�l��ݒ�	*/
					}
					oilpAd.sts = PV_STS_NORMAL;										/* 3-1-3	: OIL-P�d��A/D�l�f�[�^��ԂɊm���ݒ�						*/
				} else {
					oilpAd.dt = PV_OILPAD_MIN;
					oilpAd.sts = PV_STS_UNKNOWN;
				}
				break;
			case AD_STS_ABNORMAL:													/* 3-2		: A/D�ϊ���Ԃ��y�ُ�z�̏ꍇ								*/
				oilpAd.dt = PV_OILPAD_MIN;											/* 			: OIL-P�d��A/D�l��OIL-P�d��A/D�l:�ŏ��l��ݒ�				*/
				oilpAd.sts = PV_STS_ERR;											/* 			: OIL-P�d��A/D�l�f�[�^��ԂɈُ��ݒ�						*/
				break;
			case AD_STS_UNKNOWN:													/* 3-3		: A/D�ϊ���Ԃ��y���m��z�̏ꍇ								*/
				/*	Fall Through	*/
			default:																/* 3-4		: A/D�ϊ���Ԃ���L�ȊO�̏ꍇ								*/
				oilpAd.dt = PV_OILPAD_MIN;											/* 3-3,3-4	: OIL-P�d��A/D�l��OIL-P�d��A/D�l:�ŏ��l��ݒ�				*/
				oilpAd.sts = PV_STS_UNKNOWN;										/* 			: OIL-P�d��A/D�l�f�[�^��Ԃɖ��m���ݒ�					*/
				break;
			}
		} else {
			oilpAd.dt = PV_OILPAD_MIN;
			oilpAd.sts = PV_STS_UNKNOWN;
		}

	}else{																			/* 4		: �T���v�����O�J�n�۔��肪��L�ȊO�̏ꍇ					*/
		oilpAd.dt = PV_OILPAD_MIN;													/* 			: OIL-P�d��A/D�l��OIL-P�d��A/D�l:�ŏ��l��ݒ�				*/
		oilpAd.sts = PV_STS_UNKNOWN;												/* 			: OIL-P�d��A/D�l�f�[�^��Ԃɖ��m���ݒ�					*/
	}

	Rte_IWrite_rbl_in_oilp_pp_srIf_pv_PvOilpAd_struct(&oilpAd);						/*			: �����l��RTE�֏o��											*/
}

#define swc_in_oilp_STOP_SEC_CODE
#include "swc_in_oilp_MemMap.h"


#define swc_in_oilp_START_SEC_CODE
#include "swc_in_oilp_MemMap.h"

/********************************************************************************/
/*	�֐��T�v�F�T���v�����O�J�n�۔��菈��										*/
/*	���������F																	*/
/*																				*/
/*	����	�F����																*/
/*	�߂�l	�F�T���v�����O�J�n�۔���											*/
/*	���ӎ����F����																*/
/********************************************************************************/
static uint8 jdgSampSts(void)
{
	uint8 sampSts;							/* �T���v�����O�J�n�۔���				*/

	sampSts = OILPAD_SAMPSTS_NG;												/* 1	: �T���v�����O�J�n�۔���ɃT���v�����O�ۂ�ݒ�				*/

	if(igvAdSts != IGV_IN_AD_ST_INACTIVE){										/* 3	: IG�d��A/D�l�f�[�^��Ԃ��y���m��z�ȊO�̏ꍇ					*/
		sampSts = OILPAD_SAMPSTS_OK;											/* 		: �T���v�����O�J�n�۔���ɃT���v�����O��ݒ�				*/
	}

	return sampSts;																/* 4	: �T���v�����O�J�n�۔����ԋp								*/
}

#define swc_in_oilp_STOP_SEC_CODE
#include "swc_in_oilp_MemMap.h"


#define swc_in_oilp_START_SEC_CODE
#include "swc_in_oilp_MemMap.h"

/********************************************************************************/
/*	�֐��T�v�FIG�d������AD�l�Z�o����											*/
/*	���������F																	*/
/*																				*/
/*	����	�F�T���v�����O�J�n�۔���											*/
/*	�߂�l	�FIG�d������AD�l													*/
/*	���ӎ����F����																*/
/********************************************************************************/
static uint16 calcIgAdAvrgData(uint8 sampSts)
{
	uint16		   igAdAvrg;						/* IG�d������AD�l						*/
	uint16		   igAd;							/* IG�d��A/D�l							*/
	uint8		   tmrSts;							/* �^�C�}���							*/
	uint8		   cnt;								/* ���[�v�J�E���^						*/
	uint32		   work=0;							/* ���Z���ʊi�[�p������					*/
	Std_ReturnType ret;								/* csIf���{����							*/

	if(sampSts == OILPAD_SAMPSTS_NG){											/* 1,3,7			: �T���v�����O�J�n�۔��肪�y�T���v�����O�ہz�̏ꍇ		*/
		oilpAdIgAdAvrgSts = OILPAD_IGAD_STS_UNKNOWN;							/* 1				: IG�d������AD�l��Ԃ�IG�d��AD�l���:���m���Ԃ�ݒ�		*/

		for(cnt=0; cnt<OILPAD_IGAD_AVRG_CNT; cnt++){							/* 3				: IG�d������AD�l�o�b�t�@�̏�����							*/
			oilpAdIgAdAvrgBuf[cnt] = OILPAD_IGAD_INIT;
		}
		oilpAdIgAdAccumuCnt = OILPAD_IGAD_AVRG_INIT_CNT;						/* 					: IG�d������AD�ݐσJ�E���^�̏�����							*/
		igAdAvrg = OILPAD_IGAD_INIT;											/* 					: IG�d������AD�l��IG�d������AD�l:�����l��ݒ�				*/

	}else{																		/* 2,4,5,6			: �T���v�����O�J�n�۔��肪�y�T���v�����O�z�̏ꍇ		*/
		igAd = igvAd + OILPAD_IG_OFFSET_AD;										/* 4,5				: IG�d��A/D�l���擾											*/

		if(oilpAdIgAdAvrgSts == OILPAD_IGAD_STS_UNKNOWN){						/* 2-1,4-1,6-1		: IG�d������AD�l��Ԃ��yIG�d��AD�l���:���m���ԁz�̏ꍇ	*/
			oilpAdIgAdAvrgSts = OILPAD_IGAD_STS_SIMPLE_AVRG;					/* 2-1				: IG�d������AD�l��Ԃ�IG�d��AD�l���:�P�����Ϗ�Ԃ�ݒ�		*/

			for(cnt=0; cnt<OILPAD_IGAD_AVRG_CNT; cnt++){						/* 4-1-1 			: IG�d�����ω��񐔕��C�ȉ��̏��������{						*/
				oilpAdIgAdAvrgBuf[cnt] = igAd;									/* 					: IG�d������AD�l�o�b�t�@��IG�d��A/D�l��ݒ�					*/
			}
			oilpAdIgAdAccumuCnt = OILPAD_IGAD_AVRG_INIT_CNT;					/* 4-1-2			: IG�d������AD�ݐσJ�E���^��IG�d���~�σJ�E���^�����l��ݒ�	*/
			igAdAvrg = igAd;													/* 4-1-3			: IG�d������AD�l��IG�d��A/D�l��ݒ�							*/

																				/* 6-1				: IG�d�����ω��T���v�����O���ԗp�J�n�v�����������{			*/
			ret = Rte_Call_rp_csIf_timer_startTimer_op(TIMID_IN_OILP_IGAD, TM_TIMTYP_CYCLIC, OILPAD_TMR_PERIOD);
			if(ret != RTE_E_OK){
				;
			}else{
				;
			}
		}else{																	/* 2-2,5-1,6-2		: IG�d������AD�l��Ԃ��yIG�d��AD�l���:�P�����Ϗ�ԁz�܂���	*/
																				/* 					: IG�d������AD�l��Ԃ��yIG�d��AD�l���:�ړ����Ϗ�ԁz�̏ꍇ	*/
			ret = Rte_Call_rp_csIf_timer_sts_op(TIMID_IN_OILP_IGAD, &tmrSts);	/* 6-2				: IG�d�����ω��T���v�����O���ԗp�^�C�}��Ԃ��擾			*/
			if(ret != RTE_E_OK){
				igAdAvrg = oilpAdPreIgAdAvrg;									/* 					: �O��IG�d������AD�l�͕ێ�									*/
			}else{
				if(tmrSts == TM_TIMSTS_TIMEOUT){								/* 2-2-1,5-1-1		: �^�C�}��Ԃ��y�^�C���A�E�g�ρz�̏ꍇ						*/
					oilpAdIgAdAvrgSts = OILPAD_IGAD_STS_MOVE_AVRG;				/* 2-2-1			: IG�d������AD�l��Ԃ�IG�d��AD�l���:�ړ����Ϗ�Ԃ�ݒ�		*/

					if(oilpAdIgAdAccumuCnt >= OILPAD_IGAD_AVRG_CNT){			/* 5-1-1-2			: IG�d�����ϗݐσJ�E���^���yIG�d�����ω��񐔁z�ȏ�̏ꍇ	*/
						oilpAdIgAdAccumuCnt = OILPAD_IGAD_AVRG_INIT_CNT;		/* 					: IG�d������AD�ݐσJ�E���^��IG�d���~�σJ�E���^�����l��ݒ�	*/
					}else{														/* 5-1-1-1			: IG�d�����ϗݐσJ�E���^���yIG�d�����ω��񐔁z�����̏ꍇ	*/
						;
					}
					oilpAdIgAdAvrgBuf[oilpAdIgAdAccumuCnt] = igAd;				/* 5-1-1-1,5-1-1-2	: IG�d������AD�l�o�b�t�@[IG�d�����ϗݐσJ�E���^]��			*/
																				/* 					: 										IG�d��A/D�l��ݒ�	*/
					oilpAdIgAdAccumuCnt++;										/* 5-1-1-1,5-1-1-2	: IG�d�����ϗݐσJ�E���^���C���N�������g					*/

					for(cnt=0; cnt<OILPAD_IGAD_AVRG_CNT; cnt++){				/* 5-1-1-3			: IG�d������AD�l�o�b�t�@[IG�d�����ϗݐσJ�E���^]��			*/
						work += oilpAdIgAdAvrgBuf[cnt];							/* 					: 							IG�d�����ω��񐔕��C���Z		*/
					}
					igAdAvrg = (uint16)(work / (uint32)OILPAD_IGAD_AVRG_CNT);	/* 5-1-1-4			: IG�d������AD�l�ɏ��Z���ʂ�ݒ�							*/

				}else{															/* 2-2-2,5-1-2		: �^�C�}��Ԃ���L�ȊO�̏ꍇ								*/
																				/* 2-2-2			: IG�d������AD�l��Ԃ͕ێ�									*/
					igAdAvrg = oilpAdPreIgAdAvrg;								/* 5-1-2			: �O��IG�d������AD�l�͕ێ�									*/
				}
			}
		}
	}

	oilpAdPreIgAdAvrg = igAdAvrg;												/* 7,8				: �O��IG�d������AD�l��IG�d������AD�l��ݒ�					*/

	return igAdAvrg;															/* 9				: IG�d������AD�l��ԋp										*/
}

#define swc_in_oilp_STOP_SEC_CODE
#include "swc_in_oilp_MemMap.h"


#define swc_in_oilp_START_SEC_CODE
#include "swc_in_oilp_MemMap.h"

/********************************************************************************/
/*	�֐��T�v�F�Z���_�d���␳����												*/
/*	���������F																	*/
/*																				*/
/*	����	�F�������FIG�d������AD�l											*/
/*			�F�������F�I�C���v���b�V���Z���_�d��								*/
/*	�߂�l	�F�␳�I�C���v���b�V���Z���_�d��AD�l								*/
/*	���ӎ����F����																*/
/********************************************************************************/
static uint16 correctOilpAd(uint16 igAdAvrg, uint16 oilpAd)
{
	uint16			 correctResult;					/* �␳�I�C���v���b�V���Z���_�d��AD�l	*/
	uint32			 temp;							/* �����v�Z�p�i�[������					*/
	uint32			 stcDivisor;					/* �����i�[������						*/
	uint32			 work;							/* �␳���ʈꎞ�i�[������				*/

	if(igAdAvrg >= OILPAD_BATV_ADJ){											/* 2	: IG�d������AD�l���y�d���␳臒l�z�ȏ�̏ꍇ					*/
		temp = (uint32)(OILPAD_COEFFICIENT_A1 * igAdAvrg);						/* 		: �I�C���v���b�V���Z���_�d���C���x����␳�W���C				*/
		if(OILPAD_COEFFICIENT_B1 > temp){										/* 		: �d���␳�W��A1�C�d���␳�W��B1�CIG�d������AD�l���			*/
			stcDivisor = OILPAD_COEFFICIENT_B1 - temp;							/* 		: �␳�I�C���v���b�V���Z���_�d��AD�l���Z�o						*/
		}else{
			stcDivisor = 0;
		}
	}else{																		/* 3	: IG�d������AD�l����L�ȊO�̏ꍇ								*/
		temp = (uint32)(OILPAD_COEFFICIENT_A2 * igAdAvrg);						/* 		: �I�C���v���b�V���Z���_�d���C���x����␳�W���C				*/
		if(OILPAD_COEFFICIENT_B2 > temp){										/* 		: �d���␳�W��A2�C�d���␳�W��B2�CIG�d������AD�l���			*/
			stcDivisor = OILPAD_COEFFICIENT_B2 - temp;							/* 		: �␳�I�C���v���b�V���Z���_�d��AD�l���Z�o						*/
		}else{
			stcDivisor = 0;
		}
	}

	if(stcDivisor > 0){															/* 2�C3	: �I�C���v���b�V���Z���_�d���C���x����␳�W���C				*/
		work = (oilpAd * OILPAD_CORRECT_COEFFICIENT) / stcDivisor;				/* 		: �d���␳�W��A1/A2�C�d���␳�W��B1/B2�CIG�d������AD�l���		*/
		if(work > (uint32)OILP_UINT16_MAX){										/* 		: �␳�I�C���v���b�V���Z���_�d��AD�l���Z�o						*/
			correctResult = OILP_UINT16_MAX;
		}else{
			correctResult = (uint16)work;
		}
	}else{
		correctResult = OILP_UINT16_MAX;
	}

	return correctResult;														/* 4	: �␳�I�C���v���b�V���Z���_�d��AD�l��ԋp						*/
}

#define swc_in_oilp_STOP_SEC_CODE
#include "swc_in_oilp_MemMap.h"
