export default function user(state : any = {}, action : any) {
    const { type, payload } = action;
    switch (type){
        case 'GET_USER':
            return {...state, ...payload};
        default: return state;
    }
}