/*
	Dummy.c
*/
/*------------------------------------------------------------------------------*/
/*	インクルードファイル														*/
/*------------------------------------------------------------------------------*/
#include "ut_dummy.h"

CONSTP2CONST(struct Rte_CDS_swc_in_oilp, RTE_CONST, RTE_APPL_CONST) Rte_Inst_swc_in_oilp;

uint32 _udivi;

/*------------------------------------------------------------------------------*/
/*	ダミー関数																	*/
/*------------------------------------------------------------------------------*/

/* s03, s04 */
static uint8 dmy_Rte_Mode_swc_in_oilp_rp_msIf_ig_ModeDeclGroup_ig_Cnt;
static uint8 dmy_Rte_Mode_swc_in_oilp_rp_msIf_ig_ModeDeclGroup_ig_Ret;
FUNC(uint8, RTE_CODE) Rte_Mode_swc_in_oilp_rp_msIf_ig_ModeDeclGroup_ig(void)
{
	dmy_Rte_Mode_swc_in_oilp_rp_msIf_ig_ModeDeclGroup_ig_Cnt++;
	return dmy_Rte_Mode_swc_in_oilp_rp_msIf_ig_ModeDeclGroup_ig_Ret;
}

/* s05,s06 */
static uint8 dmy_Rte_Call_swc_in_oilp_rp_csIf_ioHwAb_normalAdcStsGet_op_Cnt;
static uint8 dmy_Rte_Call_swc_in_oilp_rp_csIf_ioHwAb_normalAdcStsGet_op_Ret;
static uint8 dmy_Rte_Call_swc_in_oilp_rp_csIf_ioHwAb_normalAdcStsGet_op_sts;
static uint32 dmy_Rte_Call_swc_in_oilp_rp_csIf_ioHwAb_normalAdcStsGet_op_id;
FUNC(uint8, RTE_CODE) Rte_Call_swc_in_oilp_rp_csIf_ioHwAb_normalAdcStsGet_op(uint32 id, P2VAR(uint8, AUTOMATIC, RTE_APPL_DATA) sts)
{
	dmy_Rte_Call_swc_in_oilp_rp_csIf_ioHwAb_normalAdcStsGet_op_Cnt++;
	*sts = dmy_Rte_Call_swc_in_oilp_rp_csIf_ioHwAb_normalAdcStsGet_op_sts;
	dmy_Rte_Call_swc_in_oilp_rp_csIf_ioHwAb_normalAdcStsGet_op_id = id;
	return dmy_Rte_Call_swc_in_oilp_rp_csIf_ioHwAb_normalAdcStsGet_op_Ret;
}

/* s05,s06 */
static uint8 dmy_Rte_Call_swc_in_oilp_rp_csIf_ioHwAb_normalAdcValGet_op_Cnt;
static uint8 dmy_Rte_Call_swc_in_oilp_rp_csIf_ioHwAb_normalAdcValGet_op_Ret;
static uint16 dmy_Rte_Call_swc_in_oilp_rp_csIf_ioHwAb_normalAdcValGet_op_val;
static uint32 dmy_Rte_Call_swc_in_oilp_rp_csIf_ioHwAb_normalAdcValGet_op_id;
FUNC(uint8, RTE_CODE) Rte_Call_swc_in_oilp_rp_csIf_ioHwAb_normalAdcValGet_op(uint32 id, P2VAR(uint16, AUTOMATIC, RTE_APPL_DATA) val)
{
	dmy_Rte_Call_swc_in_oilp_rp_csIf_ioHwAb_normalAdcValGet_op_Cnt++;
	*val = dmy_Rte_Call_swc_in_oilp_rp_csIf_ioHwAb_normalAdcValGet_op_val;
	dmy_Rte_Call_swc_in_oilp_rp_csIf_ioHwAb_normalAdcValGet_op_id = id;
	return dmy_Rte_Call_swc_in_oilp_rp_csIf_ioHwAb_normalAdcValGet_op_Ret;
}

/* s08 */
static uint8 dmy_Rte_Call_swc_in_oilp_rp_csIf_timer_startTimer_op_Cnt;
static uint8 dmy_Rte_Call_swc_in_oilp_rp_csIf_timer_startTimer_op_Ret;
static uint16 dmy_Rte_Call_swc_in_oilp_rp_csIf_timer_startTimer_op_id;
static uint8 dmy_Rte_Call_swc_in_oilp_rp_csIf_timer_startTimer_op_type;
static uint16 dmy_Rte_Call_swc_in_oilp_rp_csIf_timer_startTimer_op_period;
FUNC(Std_ReturnType, RTE_CODE) Rte_Call_swc_in_oilp_rp_csIf_timer_startTimer_op(uint16 id, uint8 type, uint16 period)
{
	dmy_Rte_Call_swc_in_oilp_rp_csIf_timer_startTimer_op_Cnt++;

	dmy_Rte_Call_swc_in_oilp_rp_csIf_timer_startTimer_op_id = id;
	dmy_Rte_Call_swc_in_oilp_rp_csIf_timer_startTimer_op_type = type;
	dmy_Rte_Call_swc_in_oilp_rp_csIf_timer_startTimer_op_period = period;

	return dmy_Rte_Call_swc_in_oilp_rp_csIf_timer_startTimer_op_Ret;
}

/* s08 */
static uint8 dmy_Rte_Call_swc_in_oilp_rp_csIf_timer_sts_op_Cnt;
static uint8 dmy_Rte_Call_swc_in_oilp_rp_csIf_timer_sts_op_Ret;
static uint16 dmy_Rte_Call_swc_in_oilp_rp_csIf_timer_sts_op_id;
static uint8 dmy_Rte_Call_swc_in_oilp_rp_csIf_timer_sts_op_sts;
FUNC(Std_ReturnType, RTE_CODE) Rte_Call_swc_in_oilp_rp_csIf_timer_sts_op(uint16 id, P2VAR(uint8, AUTOMATIC, RTE_APPL_DATA) sts)
{
	dmy_Rte_Call_swc_in_oilp_rp_csIf_timer_sts_op_Cnt++;

	dmy_Rte_Call_swc_in_oilp_rp_csIf_timer_sts_op_id = id;
	*sts = dmy_Rte_Call_swc_in_oilp_rp_csIf_timer_sts_op_sts;

	return dmy_Rte_Call_swc_in_oilp_rp_csIf_timer_sts_op_Ret;
}

