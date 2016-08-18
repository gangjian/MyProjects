/*	$RCSfile: Rte_swc_in_oilp_map.h $															*/
/*	$Date: 2015/11/26 20:56:22JST $																*/
/*	$Revision: 1.2 $																			*/
/*	 EXPLANATION: �I�C���v���b�V���Q�[�W(�w�j�\��)(����) �R���t�B�M�����[�V�����w�b�_�t�@�C��	*/

#ifndef RTE_SWC_IN_OILP_MAP_H
#define RTE_SWC_IN_OILP_MAP_H

/*------------------------------------------------------------------------------*/
/*	�C���N���[�h�t�@�C��														*/
/*------------------------------------------------------------------------------*/


/*------------------------------------------------------------------------------*/
/*	�C���N���[�h�t�@�C�� (�}�C�R�����̂ݕύX��)									*/
/*------------------------------------------------------------------------------*/

/*------------------------------------------------------------------------------*/
/*	�}�N����`�i�ύX�֎~�j														*/
/*------------------------------------------------------------------------------*/

/*------------------------------------------------------------------------------*/
/*	�R���t�B�M�����[�V�����ݒ�													*/
/*------------------------------------------------------------------------------*/
/* IG�d�����莞�Ԑݒ� */
#define IGV_STABILITY_PERIOD			((uint16)36)							/* IG�d�����莞��(400ms)						:uint16 */

/* �d���␳ */
#define OILPAD_BATV_ADJ					((uint16)650)							/* �d���␳臒l									:uint16 */
#define OILPAD_COEFFICIENT_A1			((uint16)56)							/* �d���␳�W��A1								:uint16 */
#define OILPAD_COEFFICIENT_B1			((uint32)101895)						/* �d���␳�W��B1								:uint32 */
#define OILPAD_COEFFICIENT_A2			((uint16)100)							/* �d���␳�W��A2								:uint16 */
#define OILPAD_COEFFICIENT_B2			((uint32)130271)						/* �d���␳�W��B2								:uint32 */

/* IG�d��A/D�l */
#define OILPAD_IG_OFFSET_AD				((uint16)0)								/* �_�C�I�[�h�ɂ��d���~������A/D�l			:uint16 */

/* �^�C�}�J�n�v�� */
#define OILPAD_TMR_PERIOD				((uint16)80)							/* IG�d�����ω��T���v�����O����					:uint16 */

/* �G���W����]��(�t�F�[������3s) */
#define ENGONOFF3S_IN_NE1_ON_LIMIT		((uint16)512)							/* �G���W���n�m����臒l							:uint16 */
#define ENGONOFF3S_IN_NE1_OFF_LIMIT		((uint16)256)							/* �G���W���n�e�e����臒l						:uint16 */

/* �G���W����]���ݒ� */
#define ENGONOFF3S_CM_NE1_UNUSED		((uint8)0x00)							/* ��K�p										:uint8  */
#define ENGONOFF3S_CM_NE1_USED			((uint8)0x01)							/* �K�p 										:uint8  */
#define ENGONOFF3S_GET_CM_NE1()			(ENGONOFF3S_CM_NE1_USED)				/* �G���W����]���ݒ�擾����					:uint8  */

#endif /* RTE_SWC_IN_OILP_MAP_H */
