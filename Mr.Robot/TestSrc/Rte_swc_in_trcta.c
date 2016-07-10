/*	$RCSfile: Rte_swc_in_trcta.c $									*/
/*	$Date: 2015/11/05 16:43:06JST $									*/
/*	$Revision: 1.2 $												*/
/*	 EXPLANATION: [RCTA(�e���e�[��)](����) �\�[�X�t�@�C��			*/
/** 
 * \file Rte_swc_in_trcta.c
 *
 * \brief <Add a description here>
 *
 * \b Generator: Picea Rte V4.9.0-Delivery-Build275
 */

/*============================================================================*
 * PREPROCESSOR DIRECTIVES                                                    *
 *============================================================================*/

/* INCLUDE DIRECTIVES FOR OTHER HEADERS --------------------------------------*/

#include "Rte_swc_in_trcta.h"

/*============================================================================*
 * PREPROCESSOR DIRECTIVES                                                    *
 *============================================================================*/

/* INCLUDE DIRECTIVES FOR OTHER HEADERS --------------------------------------*/

#include "share_lib_can_in_step.h"			/* CAN�W������(step)���ʃ��C�u���� */

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
 * - cmp_in_TRCTA (uint8)
 *   - IN_TRCTA_OFF
 *   - IN_TRCTA_ON
 * - cmp_pv_PvRctasw (uint8)
 *   - PV_RCTASW_IN_OFF
 *   - PV_RCTASW_IN_ON
 *   - PV_RCTASW_IN_UNFIX
 *
 *----------------------------------------------------------------------------*/
/*	�o�͒l�e�[�u���̃T�C�Y	*/
#define IN_RCTASW_TBL_SIZE			((uint8) 3)

/*============================================================================*
 * EXPORTED TYPEDEF DECLARATIONS                                              *
 *============================================================================*/

/*-----------------------------------------------------------------------------/
 *
 * Used Implementation Data Types
 *  uint8
 *  uint8
 *  pvU1NoSts: Record with 1 fields.
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
 * STATIC PARAMETERS		                                                  *
 *============================================================================*/
#define swc_in_trcta_START_SEC_CONST_UNSPECIFIED
#include "swc_in_trcta_MemMap.h"

/*	�o�͒l�e�[�u��	*/
/*	RCTA_SW �o�͒l�e�[�u��	*/
static const uint8 inTblRctasw[IN_RCTASW_TBL_SIZE] =
{
	PV_RCTASW_IN_OFF,
	PV_RCTASW_IN_ON,
	PV_RCTASW_IN_UNFIX
};

/*	���͋��ʏ���(VAL)�}�l�[�W���e�[�u��	*/
static const MNG_STEP_VAL rctasw_can_mng_tbl = {
	IN_RCTASW_TBL_SIZE,								/*	���͐M���ő�l			*/
	PV_RCTASW_IN_OFF,								/*	�����l�����l			*/
	PV_RCTASW_IN_OFF,								/*	�r�⎞�����l			*/
	PV_RCTASW_IN_ON,								/*	�͈͊O�������l			*/
	&inTblRctasw[0]									/*	�ϊ��e�[�u���A�h���X	*/
};

#define swc_in_trcta_STOP_SEC_CONST_UNSPECIFIED
#include "swc_in_trcta_MemMap.h"

/*============================================================================*
 * EXPORTED FUNCTIONS PROTOTYPES                                              *
 *============================================================================*/
/* Declaration of runnable entities ------------------------------------------*/

/*-----------------------------------------------------------------------------/
 *
 * Runnable Entity: sym_rbl_in_trcta_igoff
 *  Can be invoked concurrently: TRUE
 *  Events:
 *  - Triggerred by ev_te_in_trcta_igoff (TIMING-EVENT) once every 0.01 seconds
 *    but not in mode(s): ModeDeclGroup_ig
 *
 *-----------------------------------------------------------------------------/
 *
 * Implicit Sender-Reciever Data-Write-Access API(s): 
 * - void Rte_IWrite_rbl_in_trcta_igoff_pp_srIf_pv_PvRctasw_struct(const pvU1NoSts* struct)
 * - pvU1NoSts* Rte_IWriteRef_rbl_in_trcta_igoff_pp_srIf_pv_PvRctasw_struct(void)
 *
 *----------------------------------------------------------------------------*/

#define swc_in_trcta_START_SEC_CODE
#include "swc_in_trcta_MemMap.h"

/********************************************************************************/
/*	�֐��T�v�FRCTA(�e���e�[��)(����) IG-OFF����									*/
/*	���������F																	*/
/*																				*/
/*	����	�F����																*/
/*	�߂�l	�F����																*/
/*	���ӎ����F����																*/
/********************************************************************************/
FUNC(void, swc_in_trcta_CODE) sym_rbl_in_trcta_igoff(void)
{
   /* Insert your code here! */
	pvU1NoSts	pvOut;									/*	�����l				*/

	/*	�����l������(�f�[�^)	*/
	pvOut.dt = PV_RCTASW_IN_OFF;
	Rte_IWrite_rbl_in_trcta_igoff_pp_srIf_pv_PvRctasw_struct(&pvOut);
}

#define swc_in_trcta_STOP_SEC_CODE
#include "swc_in_trcta_MemMap.h"


/*-----------------------------------------------------------------------------/
 *
 * Runnable Entity: sym_rbl_in_trcta_igon
 *  Can be invoked concurrently: TRUE
 *  Events:
 *  - Triggerred by ev_te_in_trcta_igon (TIMING-EVENT) once every 0.01 seconds
 *    but not in mode(s): ModeDeclGroup_ig, ModeDeclGroup_ig
 *
 *-----------------------------------------------------------------------------/
 *
 * Implicit Sender-Reciever Data-Read-Access API(s): 
 * - uint8 Rte_IRead_rbl_in_trcta_igon_rp_srIf_in_TRCTA_val(void)
 * - uint8 Rte_IRead_rbl_in_trcta_igon_rp_srIf_in_TRCTASts_val(void)
 *
 * Implicit Sender-Reciever Data-Write-Access API(s): 
 * - void Rte_IWrite_rbl_in_trcta_igon_pp_srIf_pv_PvRctasw_struct(const pvU1NoSts* struct)
 * - pvU1NoSts* Rte_IWriteRef_rbl_in_trcta_igon_pp_srIf_pv_PvRctasw_struct(void)
 *
 *----------------------------------------------------------------------------*/

#define swc_in_trcta_START_SEC_CODE
#include "swc_in_trcta_MemMap.h"

/********************************************************************************/
/*	�֐��T�v�FRCTA(�e���e�[��)(����) IG-ON����									*/
/*	���������F																	*/
/*																				*/
/*	����	�F����																*/
/*	�߂�l	�F����																*/
/*	���ӎ����F����																*/
/********************************************************************************/
FUNC(void, swc_in_trcta_CODE) sym_rbl_in_trcta_igon(void)
{
   /* Insert your code here! */
	VAR_IN_STEP		varInStep;							/*	���̓e�[�u��		*/
	VAR_OUT_STEP	varOutStep;							/*	�o�̓e�[�u��		*/
	pvU1NoSts		pvOut;								/*	�����l				*/

	/*	�����l�̏�����	*/
	varOutStep.pv = PV_RCTASW_IN_OFF;

	/*	���͒l�������l�ϊ�	*/
	varInStep.inSig = Rte_IRead_rbl_in_trcta_igon_rp_srIf_in_TRCTA_val();
	varInStep.msgSts = Rte_IRead_rbl_in_trcta_igon_rp_srIf_in_TRCTASts_val();
	varInStep.powerSts = SHARE_LIB_POWER_ON;
	ShareLibStepFailJudgeVal(&varInStep, &rctasw_can_mng_tbl ,&varOutStep);

	/*	�����l�o��	*/
	pvOut.dt = varOutStep.pv;
	Rte_IWrite_rbl_in_trcta_igon_pp_srIf_pv_PvRctasw_struct(&pvOut);
}

#define swc_in_trcta_STOP_SEC_CODE
#include "swc_in_trcta_MemMap.h"


#define swc_in_trcta_START_SEC_CODE
#include "swc_in_trcta_MemMap.h"

/********************************************************************************/
/*	�֐��T�v�FRCTA(�e���e�[��)(����) ����������(���Z�b�g)						*/
/*	���������F																	*/
/*																				*/
/*	����	�F����																*/
/*	�߂�l	�F����																*/
/*	���ӎ����F����																*/
/********************************************************************************/
FUNC(void, swc_in_trcta_CODE) sym_rbl_in_trcta_initReset(void)
{
   /* Insert your code here! */
	pvU1NoSts	pvOut;									/*	�����l				*/

	/*	�����l������(�f�[�^)	*/
	pvOut.dt = PV_RCTASW_IN_OFF;
	Rte_IWrite_rbl_in_trcta_initReset_pp_srIf_pv_PvRctasw_struct(&pvOut);
}

#define swc_in_trcta_STOP_SEC_CODE
#include "swc_in_trcta_MemMap.h"


#define swc_in_trcta_START_SEC_CODE
#include "swc_in_trcta_MemMap.h"

/********************************************************************************/
/*	�֐��T�v�FRCTA(�e���e�[��)(����) ����������(�E�F�C�N�A�b�v)					*/
/*	���������F																	*/
/*																				*/
/*	����	�F����																*/
/*	�߂�l	�F����																*/
/*	���ӎ����F����																*/
/********************************************************************************/
FUNC(void, swc_in_trcta_CODE) sym_rbl_in_trcta_initWakeup(void)
{
   /* Insert your code here! */
	pvU1NoSts	pvOut;									/*	�����l				*/

	/*	�����l������(�f�[�^)	*/
	pvOut.dt = PV_RCTASW_IN_OFF;
	Rte_IWrite_rbl_in_trcta_initWakeup_pp_srIf_pv_PvRctasw_struct(&pvOut);
}

#define swc_in_trcta_STOP_SEC_CODE
#include "swc_in_trcta_MemMap.h"


