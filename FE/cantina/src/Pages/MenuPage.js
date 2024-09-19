import { useEffect, useState } from "react";
import {
    Table, TableBody, TableCell, TableContainer, TableHead, TableRow,
    Paper, TableFooter, TablePagination, IconButton, Box,
    Container
} from '@mui/material';
import { FirstPage, LastPage, KeyboardArrowLeft, KeyboardArrowRight } from '@mui/icons-material';
import { fetchProducts } from '../Services/APIService';
import { initializeSignalR } from '../Services/ProductsSocketService';

function MenuPage() {
    const [products, setProducts] = useState([]);
    const [page, setPage] = useState(0);
    const [rowsPerPage, setRowsPerPage] = useState(5);

    useEffect(() => {
        const loadProducts = async () => {
            try {
                const productsData = await fetchProducts();
                setProducts(productsData);
            } catch (error) {
                console.error('Error fetching products from API!', error);
            }
        };

        loadProducts();

        const connection = initializeSignalR((updatedProduct) => {
            setProducts(prevProducts => {
                if (updatedProduct.active) {
                    const existingProductIndex = prevProducts.findIndex(p => p.id === updatedProduct.id);
                    if (existingProductIndex > -1) {
                        const updatedProducts = [...prevProducts];
                        updatedProducts[existingProductIndex] = updatedProduct;
                        return updatedProducts;
                    } else {
                        return [...prevProducts, updatedProduct];
                    }
                } else {
                    return prevProducts.filter(p => p.id !== updatedProduct.id);
                }
            });
        });

        return () => {
            connection.stop().catch(err => console.error("SignalR Disconnection Error: ", err));
        };
    }, []);

    const handleChangePage = (event, newPage) => {
        setPage(newPage);
    };

    const handleChangeRowsPerPage = (event) => {
        setRowsPerPage(parseInt(event.target.value, 10));
        setPage(0);
    };

    return (
        <Container maxWidth="lg" sx={{ marginTop: 3 }}>
            <Paper sx={{ width: '100%', overflow: 'hidden' }}>
                <TableContainer>
                    <Table>
                        <TableHead>
                            <TableRow>
                                <TableCell>Name</TableCell>
                                <TableCell>Description</TableCell>
                                <TableCell align="right">Price</TableCell>
                                <TableCell align="right">Unit</TableCell>
                                <TableCell align="right">Stock</TableCell>
                            </TableRow>
                        </TableHead>
                        <TableBody>
                            {products.slice(page * rowsPerPage, page * rowsPerPage + rowsPerPage).map((product) => (
                                <TableRow key={product.id} hover>
                                    <TableCell>{product.name}</TableCell>
                                    <TableCell>{product.description}</TableCell>
                                    <TableCell align="right">{product.price.toFixed(2)}</TableCell>
                                    <TableCell align="right">{product.unit}</TableCell>
                                    <TableCell align="right">{product.stock}</TableCell>
                                </TableRow>
                            ))}
                        </TableBody>
                        <TableFooter>
                            <TableRow>
                                <TablePagination
                                    rowsPerPageOptions={[5, 10]}
                                    colSpan={5}
                                    count={products.length}
                                    rowsPerPage={rowsPerPage}
                                    page={page}
                                    SelectProps={{
                                        inputProps: { 'aria-label': 'rows per page' },
                                        native: true,
                                    }}
                                    onPageChange={handleChangePage}
                                    onRowsPerPageChange={handleChangeRowsPerPage}
                                    ActionsComponent={TablePaginationActions}
                                />
                            </TableRow>
                        </TableFooter>
                    </Table>
                </TableContainer>
            </Paper>
        </Container>
    );
}

function TablePaginationActions(props) {
    const { count, page, rowsPerPage, onPageChange } = props;

    const handleFirstPageButtonClick = (event) => {
        onPageChange(event, 0);
    };

    const handleBackButtonClick = (event) => {
        onPageChange(event, page - 1);
    };

    const handleNextButtonClick = (event) => {
        onPageChange(event, page + 1);
    };

    const handleLastPageButtonClick = (event) => {
        onPageChange(event, Math.max(0, Math.ceil(count / rowsPerPage) - 1));
    };

    return (
        <Box sx={{ flexShrink: 0, ml: 2.5 }}>
            <IconButton onClick={handleFirstPageButtonClick} disabled={page === 0} aria-label="first page">
                <FirstPage />
            </IconButton>
            <IconButton onClick={handleBackButtonClick} disabled={page === 0} aria-label="previous page">
                <KeyboardArrowLeft />
            </IconButton>
            <IconButton onClick={handleNextButtonClick} disabled={page >= Math.ceil(count / rowsPerPage) - 1} aria-label="next page">
                <KeyboardArrowRight />
            </IconButton>
            <IconButton onClick={handleLastPageButtonClick} disabled={page >= Math.ceil(count / rowsPerPage) - 1} aria-label="last page">
                <LastPage />
            </IconButton>
        </Box>
    );
}

export default MenuPage;
